#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Unity.AI.Navigation;
using MySims;

/// <summary>
/// Generador de arte base procedural y constructor de escena jugable.
/// Menu MySims en la barra del editor:
///   1. Generar prefabs base: crea muebles, mascotas, nino, robot y pared con placeholder art.
///   2. Crear escena base: monta la escena Main completa con todos los sistemas, zonas, UI y NavMesh.
/// Correr en orden 1 y luego 2. Reemplazar el placeholder art por modelos reales despues.
/// </summary>
public static class MySimsAssetBuilder
{
    static string PrefabDir = "Assets/Generated/Prefabs";
    static string MatDir = "Assets/Generated/Materials";

    // ---------- helpers ----------

    static Material GetMat(string name, Color c)
    {
        string path = $"{MatDir}/{name}.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var mat = new Material(shader);
        mat.color = c;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static GameObject Cube(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static GameObject SavePrefab(GameObject go, string name)
    {
        string path = $"{PrefabDir}/{name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ---------- modelos Kenney ----------

    /// <summary>Instancia un modelo FBX de Assets/Art como hijo del prefab, con collider por bounds.</summary>
    static GameObject AttachModel(string folder, string modelName, Transform parent, float scale, System.Func<GameObject> fallback, Color? tint = null)
    {
        string path = $"Assets/Art/{folder}/{modelName}.fbx";
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (fbx == null)
        {
            if (fallback != null) return fallback();
            return null;
        }

        var model = Object.Instantiate(fbx);
        model.name = "modelo";
        model.transform.SetParent(parent);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one * scale;

        // Kenney ships sin textura: pintamos con el color de la paleta en vez de dejar el material gris por defecto
        if (tint.HasValue)
        {
            var tintMat = GetMat(modelName + "_tint", tint.Value);
            foreach (var r in model.GetComponentsInChildren<Renderer>()) r.sharedMaterial = tintMat;
        }

        // Collider generado por los bounds visuales para que el raycast funcione
        if (model.GetComponent<Collider>() == null)
        {
            var box = model.AddComponent<BoxCollider>();
            var bounds = new Bounds(model.transform.position, Vector3.zero);
            foreach (var r in model.GetComponentsInChildren<Renderer>()) bounds.Encapsulate(r.bounds);
            box.center = model.transform.InverseTransformPoint(bounds.center);
            Vector3 worldSize = bounds.size;
            box.size = model.transform.InverseTransformDirection(worldSize);
        }
        return model;
    }

    // ---------- muebles ----------

    static GameObject BuildFurniture(string name, NeedType need, float price, Color color, SkillType skill, Vector3 size, string kenneyModel, float modelScale, bool forPets = false)
    {
        var root = new GameObject(name);
        AttachModel("KenneyFurniture", kenneyModel, root.transform, modelScale,
            () => { var mat = GetMat(name, color); return Cube("base", root.transform, Vector3.zero, size, mat); },
            color);

        var placeable = root.AddComponent<PlaceableObject>();
        placeable.satisfies = need;
        placeable.displayName = name;
        placeable.price = price;
        placeable.trainsSkill = skill;
        placeable.skillXpPerUse = 1f;
        placeable.isForPets = forPets;

        var ip = new GameObject("interactionPoint");
        ip.transform.SetParent(root.transform);
        ip.transform.localPosition = new Vector3(0f, 0f, size.z / 2f + 0.8f);
        placeable.interactionPoint = ip.transform;
        return root;
    }


    /// <summary>Mascota low-poly estilo Kenney construida con prismas (el kit no trae animales).</summary>
    static GameObject BuildPet(string name, Color color, bool isDog, float height)
    {
        var root = new GameObject(name);
        var mat = GetMat(name, color);
        var light = GetMat(name + "_Light", Color.Lerp(color, Color.white, 0.45f));

        // Cuerpo (caja horizontal)
        Cube("body", root.transform, new Vector3(0f, height * 0.55f, 0f), new Vector3(height * 0.9f, height * 0.55f, height * 1.5f), mat);

        // Cabeza + hocico
        var head = Cube("head", root.transform, new Vector3(0f, height * 1.05f, height * 0.75f), new Vector3(height * 0.6f, height * 0.55f, height * 0.55f), mat);
        Cube("snout", root.transform, new Vector3(0f, height * 0.92f, height * 1.05f), new Vector3(height * 0.3f, height * 0.25f, height * 0.3f), light);
        // Ojos
        Cube("eyeL", root.transform, new Vector3(height * 0.18f, height * 1.15f, height * 0.98f), new Vector3(height * 0.08f, height * 0.08f, height * 0.04f), GetMat(name + "_Eye", Color.black));
        Cube("eyeR", root.transform, new Vector3(-height * 0.18f, height * 1.15f, height * 0.98f), new Vector3(height * 0.08f, height * 0.08f, height * 0.04f), GetMat(name + "_Eye", Color.black));

        // Orejas: caidas para perro, puntiagudas para gato
        if (isDog)
        {
            Cube("earL", root.transform, new Vector3(height * 0.32f, height * 1.28f, height * 0.7f), new Vector3(height * 0.14f, height * 0.3f, height * 0.1f), light);
            Cube("earR", root.transform, new Vector3(-height * 0.32f, height * 1.28f, height * 0.7f), new Vector3(height * 0.14f, height * 0.3f, height * 0.1f), light);
        }
        else
        {
            Cube("earL", root.transform, new Vector3(height * 0.3f, height * 1.42f, height * 0.62f), new Vector3(height * 0.16f, height * 0.22f, height * 0.08f), mat);
            Cube("earR", root.transform, new Vector3(-height * 0.3f, height * 1.42f, height * 0.62f), new Vector3(height * 0.16f, height * 0.22f, height * 0.08f), mat);
        }

        // Patas
        for (int i = 0; i < 4; i++)
        {
            float x = (i % 2 == 0 ? 1 : -1) * height * 0.3f;
            float z = (i < 2 ? 1 : -1) * height * 0.5f;
            Cube("leg" + i, root.transform, new Vector3(x, height * 0.14f, z), new Vector3(height * 0.18f, height * 0.28f, height * 0.18f), light);
        }

        // Cola: corta en perro, larga en gato
        if (isDog)
            Cube("tail", root.transform, new Vector3(0f, height * 0.75f, -height * 0.8f), new Vector3(height * 0.12f, height * 0.12f, height * 0.35f), light);
        else
            Cube("tail", root.transform, new Vector3(0f, height * 0.9f, -height * 0.9f), new Vector3(height * 0.1f, height * 0.1f, height * 0.8f), mat);

        // Un solo collider en la raiz: quitamos los BoxCollider de cada prisma
        foreach (var box in root.GetComponentsInChildren<BoxCollider>())
            Object.DestroyImmediate(box);
        var cap = root.AddComponent<CapsuleCollider>();
        cap.direction = 2; // eje Z (el cuerpo es horizontal)
        cap.height = height * 1.6f;
        cap.radius = height * 0.45f;
        cap.center = new Vector3(0f, height * 0.55f, 0f);
        return root;
    }

    static GameObject BuildCreature(string name, Color bodyColor, float height, string characterModel = null)

    {
        var root = new GameObject(name);

        var model = characterModel != null
            ? AttachModel("KenneyCharacters", characterModel, root.transform, height / 1.8f, null, bodyColor)
            : null;

        if (model == null)
        {
            // Respaldo en capsulas si no esta el kit de personajes
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "body";
            body.transform.SetParent(root.transform);
            body.transform.localPosition = new Vector3(0f, height / 2f, 0f);
            body.transform.localScale = new Vector3(height / 3f, height / 2f, height / 3f);
            var mat = GetMat(name, bodyColor);
            body.GetComponent<Renderer>().sharedMaterial = mat;

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "head";
            head.transform.SetParent(root.transform);
            head.transform.localPosition = new Vector3(0f, height, 0f);
            head.transform.localScale = Vector3.one * height / 3.5f;
            head.GetComponent<Renderer>().sharedMaterial = mat;
        }
        else
        {
            var cap = model.AddComponent<CapsuleCollider>();
            cap.height = height;
            cap.radius = height / 4f;
            cap.center = new Vector3(0f, height / 2f, 0f);
        }
        return root;
    }

    [MenuItem("MySims/1. Generar prefabs base")]
    public static void GeneratePrefabs()
    {
        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(MatDir);

        // Muebles con modelos low-poly CC0 de Kenney. Si falta el FBX, respaldo en cubo.
        SavePrefab(BuildFurniture("Cama",       NeedType.Energia,   150f, new Color(0.9f, 0.9f, 0.95f), SkillType.Creatividad, new Vector3(1f, 0.5f, 2f),    "bedSingle",        1f), "Cama");
        SavePrefab(BuildFurniture("Refri",      NeedType.Hambre,    200f, new Color(0.85f, 0.85f, 0.9f),  SkillType.Cocina,     new Vector3(0.8f, 1.8f, 0.8f), "kitchenFridge",    1f), "Refri");
        SavePrefab(BuildFurniture("Ducha",      NeedType.Higiene,   120f, new Color(0.5f, 0.8f, 0.95f),  SkillType.Fitness,    new Vector3(0.9f, 2f, 0.9f),   "shower",           1f), "Ducha");
        SavePrefab(BuildFurniture("TV",         NeedType.Diversión, 250f, new Color(0.15f, 0.15f, 0.18f), SkillType.Logica,     new Vector3(1.2f, 0.8f, 0.2f), "televisionModern", 1f), "TV");
        SavePrefab(BuildFurniture("Sofa",       NeedType.Social,    90f,  new Color(0.6f, 0.3f, 0.3f),    SkillType.Carisma,    new Vector3(1.6f, 0.6f, 0.8f), "loungeSofa",       1f), "Sofa");
        SavePrefab(BuildFurniture("Escritorio", NeedType.Diversión, 130f, new Color(0.5f, 0.35f, 0.2f),  SkillType.Logica,     new Vector3(1.2f, 0.75f, 0.7f), "desk",             1f), "Escritorio");
        SavePrefab(BuildFurniture("Libreria",   NeedType.Diversión,  80f, new Color(0.55f, 0.4f, 0.25f),  SkillType.Logica,     new Vector3(1f, 1.8f, 0.4f),   "bookcaseOpen",     1f), "Libreria");
        SavePrefab(BuildFurniture("Caballete",  NeedType.Diversión, 100f, new Color(0.8f, 0.7f, 0.4f),  SkillType.Creatividad, new Vector3(0.6f, 1.5f, 0.6f), null,               1f), "Caballete");
        SavePrefab(BuildFurniture("Planta",     NeedType.Diversión,  45f, new Color(0.3f, 0.6f, 0.3f),   SkillType.Creatividad, new Vector3(0.5f, 0.8f, 0.5f), "pottedPlant",     1f), "Planta");
        SavePrefab(BuildFurniture("Comedero",  NeedType.Hambre,    40f,  new Color(0.4f, 0.25f, 0.15f), SkillType.Cocina,     new Vector3(0.5f, 0.2f, 0.5f), null,               1f, true), "Comedero");

        // Pared para el editor de casa
        var wall = new GameObject("Pared");
        var wmat = GetMat("Pared", new Color(0.92f, 0.92f, 0.88f));
        Cube("segment", wall.transform, Vector3.zero, new Vector3(0.2f, 2.5f, 1f), wmat);
        wall.AddComponent<WallPiece>();
        SavePrefab(wall, "Pared");

        // Criaturas: personajes low-poly de Kenney con respaldo en capsula
        var perro = BuildPet("Perro", new Color(0.55f, 0.35f, 0.2f), true, 0.5f);
        perro.AddComponent<Pet>().species = Pet.Species.Perro;
        SavePrefab(perro, "Perro");

        var gato = BuildPet("Gato", new Color(0.9f, 0.85f, 0.7f), false, 0.3f);
        gato.AddComponent<Pet>().species = Pet.Species.Gato;
        SavePrefab(gato, "Gato");

        var nino = BuildCreature("Child", new Color(0.95f, 0.75f, 0.6f), 1.1f, "character-k");
        SavePrefab(nino, "Child");

        var robot = BuildCreature("Robot", new Color(0.75f, 0.8f, 0.85f), 1.4f, "character-r");
        robot.AddComponent<RobotCompanion>();
        SavePrefab(robot, "Robot");

        // Playas: actividades especiales (nadar, bronceado, volley)
        SavePrefab(BuildFurniture("Alberca",      NeedType.Diversión, 300f, new Color(0.3f, 0.65f, 0.9f),  SkillType.Fitness,     new Vector3(2.5f, 0.6f, 2.5f), null, 1f), "Alberca");
        SavePrefab(BuildFurniture("SillaPlaya",   NeedType.Energia,    60f, new Color(1f, 0.95f, 0.75f),    SkillType.Creatividad, new Vector3(0.6f, 0.5f, 1.3f),  null, 1f), "SillaPlaya");
        SavePrefab(BuildFurniture("RedVoleibol",  NeedType.Diversión, 150f, new Color(0.9f, 0.75f, 0.4f),  SkillType.Carisma,    new Vector3(2f, 2.2f, 1f),      null, 1f), "RedVoleibol");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MySims: prefabs base generados en " + PrefabDir + ". Ya puedes correr MySims/2. Crear escena base.");
    }

    // ---------- escena ----------

    [MenuItem("MySims/2. Crear escena base")]
    public static void BuildScene()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Cama.prefab") == null)
        {
            Debug.LogError("Corre primero MySims/1. Generar prefabs base");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Directory.CreateDirectory("Assets/Scenes");

        // Suelo con NavMesh
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Suelo";
        ground.transform.localScale = new Vector3(5f, 1f, 5f);
        ground.GetComponent<Renderer>().sharedMaterial = GetMat("Suelo", new Color(0.45f, 0.62f, 0.4f));
        var surface = ground.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.All;

        // Presentacion: cielo, sol con sombras suaves, casa inicial, arboles y nubes
        BeautifyScene();
        BuildStarterHouse();
        BuildTree(new Vector3(-13f, 0f, 6f), 1.2f);
        BuildTree(new Vector3(14f, 0f, 9f), 1.5f);
        BuildTree(new Vector3(16f, 0f, -12f), 1.1f);
        BuildTree(new Vector3(-17f, 0f, -3f), 1.4f);
        BuildTree(new Vector3(10f, 0f, 13f), 1.2f);
        BuildTree(new Vector3(-9f, 0f, 15f), 1.0f);
        BuildCloud(new Vector3(-15f, 26f, -10f), 1.3f);
        BuildCloud(new Vector3(20f, 29f, 15f), 1.8f);
        BuildCloud(new Vector3(5f, 31f, -25f), 1.5f);
        BuildCloud(new Vector3(-25f, 28f, 20f), 1.1f);
        BuildCloud(new Vector3(12f, 27f, 28f), 1.4f);

        // Personaje jugador
        var player = BuildCreature("Sim", new Color(0.85f, 0.66f, 0.5f), 1.8f, "character-a");
        player.name = "Jugador";
        player.AddComponent<NavMeshAgent>();
        var npc = player.AddComponent<NPCController>();
        npc.isPlayerControlled = true;
        player.AddComponent<SimpleLocomotion>();
        var customizer = player.AddComponent<CharacterCustomizer>();
        customizer.bodyRenderer = player.GetComponentInChildren<Renderer>();
        player.AddComponent<NeedsSystem>();

        // GameManager con todos los sistemas
        var gm = new GameObject("GameManager");
        var manager = gm.AddComponent<GameManager>();
        manager.playerCharacter = npc;
        manager.playerNeeds = player.GetComponent<NeedsSystem>();

        var time = gm.AddComponent<TimeSystem>();
        gm.AddComponent<CalendarSystem>();
        var economy = gm.AddComponent<EconomySystem>();
        var jobs = gm.AddComponent<JobSystem>();
        var uni = gm.AddComponent<UniversitySystem>();
        var school = gm.AddComponent<HighSchoolSystem>();
        gm.AddComponent<BusinessSystem>();
        gm.AddComponent<FameSystem>();
        gm.AddComponent<SupernaturalSystem>();
        gm.AddComponent<HobbyClubSystem>();
        gm.AddComponent<HousingSystem>();
        gm.AddComponent<EcoSystem>();
        gm.AddComponent<FarmSystem>();
        var afterlife = gm.AddComponent<AfterlifeSystem>();
        afterlife.permadeathEnabled = false; // se puede activar desde el Inspector
        gm.AddComponent<AchievementSystem>();
        gm.AddComponent<SkillSystem>();
        var dialogue = gm.AddComponent<DialogueSystem>();
        gm.AddComponent<FamilySystem>();
        var pets = gm.AddComponent<PetSystem>();
        var future = gm.AddComponent<FutureSystem>();
        gm.AddComponent<JobCatalog>();
        gm.AddComponent<SeasonSystem>();
        gm.AddComponent<AudioManager>();
        gm.AddComponent<DailyMissionSystem>();

        var workPoint = new GameObject("PuntoTrabajo");
        workPoint.transform.position = new Vector3(15f, 0f, 0f);
        jobs.workPoint = workPoint.transform;

        var campus = new GameObject("PuntoUniversidad");
        campus.transform.position = new Vector3(-15f, 0f, 0f);
        uni.campusPoint = campus.transform;

        var schoolPoint = new GameObject("PuntoEscuela");
        schoolPoint.transform.position = new Vector3(-15f, 0f, 15f);
        school.schoolPoint = schoolPoint.transform;

        // PrefabRegistry con todo el catalogo generado
        var registry = gm.AddComponent<PrefabRegistry>();
        string[] names = { "Cama", "Refri", "Ducha", "TV", "Sofa", "Escritorio", "Caballete", "Comedero", "Alberca", "SillaPlaya", "RedVoleibol", "Pared", "Perro", "Gato", "Child", "Robot" };
        foreach (var n in names)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{n}.prefab");
            if (prefab != null) registry.entries.Add(new PrefabRegistry.Entry { prefab = prefab });
        }
        pets.perroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Perro.prefab");
        pets.gatoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Gato.prefab");
        future.robotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Robot.prefab");

        // Construccion
        var buildGo = new GameObject("ModoConstruccion");
        var grid = buildGo.AddComponent<GridPlacement>();
        grid.groundMask = ~0;
        grid.blockingMask = 0;
        var walls = buildGo.AddComponent<WallBuilder>();
        walls.groundMask = ~0;
        walls.wallMask = 0;
        walls.wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Pared.prefab");

        // Camara: control orbital y input tactil
        var cam = Camera.main;
        var orbit = cam.gameObject.AddComponent<TouchCameraController>();
        orbit.followTarget = player.transform;
        orbit.distance = 14f;
        orbit.SnapToTarget(); // encuadra ya mismo, sin esperar al suavizado de Play
        var input = cam.gameObject.AddComponent<TouchInputController>();
        input.playerCharacter = npc;
        input.groundMask = ~0;
        input.objectMask = ~0;

        // Zonas del mundo: Monterrey activa, Cancun y NeoCiudad inactivas
        MakeZone("Monterrey", "monterrey", "Mexico", LocationType.Ciudad, new Vector3(0f, 0f, 0f), new Color(0.45f, 0.62f, 0.4f), true);
        var cancun = MakeZone("Cancun", "cancun", "Mexico", LocationType.Playa, new Vector3(100f, 0f, 0f), new Color(0.95f, 0.85f, 0.6f), false);
        var neo = MakeZone("NeoCiudad", "neociudad", "Futuro", LocationType.Ciudad, new Vector3(200f, 0f, 0f), new Color(0.35f, 0.4f, 0.5f), false);

        // Actividad de playa en Cancun
        var snorkelGo = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Cama.prefab"); // placeholder visual
        var snorkel = Object.Instantiate(snorkelGo, cancun.zoneRoot.transform);
        snorkel.name = "Snorkel";
        snorkel.AddComponent<VacationActivity>().activityType = VacationActivityType.Snorkel;

        // Actividades de playa: nadar, bronceado y volley dentro de Cancun
        PlaceInZone(cancun, "Alberca", new Vector3(-4f, 0f, 2f));
        PlaceInZone(cancun, "SillaPlaya", new Vector3(1f, 0f, 3f));
        PlaceInZone(cancun, "SillaPlaya", new Vector3(3f, 0f, 3f));
        PlaceInZone(cancun, "RedVoleibol", new Vector3(0f, 0f, -3f));

        // Palmeras de la playa (hijas de la zona para que viajen con ella)
        BuildPalm(cancun.zoneRoot.transform, new Vector3(-11f, 0f, -7f), 1.1f);
        BuildPalm(cancun.zoneRoot.transform, new Vector3(8f, 0f, -9f), 1.3f);
        BuildPalm(cancun.zoneRoot.transform, new Vector3(11f, 0f, 4f), 1.0f);
        BuildPalm(cancun.zoneRoot.transform, new Vector3(-12f, 0f, 5f), 1.2f);
        BuildPalm(cancun.zoneRoot.transform, new Vector3(7f, 0f, 11f), 1.1f);

        // Vecinos con rutina en Monterrey
        var vecino1 = BuildCreature("VecinoA", new Color(0.7f, 0.5f, 0.35f), 1.8f, "character-b");
        vecino1.AddComponent<NavMeshAgent>();
        vecino1.AddComponent<NPCController>().isPlayerControlled = false;
        var r1 = vecino1.AddComponent<NPCRoutine>();
        r1.npcName = "Vecino A";

        var vecino2 = BuildCreature("VecinoB", new Color(0.55f, 0.4f, 0.3f), 1.8f, "character-c");
        vecino2.AddComponent<NavMeshAgent>();
        vecino2.AddComponent<NPCController>().isPlayerControlled = false;
        var r2 = vecino2.AddComponent<NPCRoutine>();
        r2.npcName = "Vecino B";

        // Muebles iniciales en Monterrey
        PlaceInScene("Cama", new Vector3(-5f, 0f, -5f));
        PlaceInScene("Refri", new Vector3(5f, 0f, -5f));
        PlaceInScene("TV", new Vector3(0f, 0f, -7f));

        // Mascota inicial
        var perroGo = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Perro.prefab"));
        perroGo.transform.position = new Vector3(2f, 0f, 2f);

        // NavMesh de la zona activa
        surface.BuildNavMesh();

        // UI basica: barras de necesidad y botones de velocidad
        BuildUI(manager, dialogue);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        Debug.Log("MySims: escena base creada en Assets/Scenes/Main.unity. Dale Play para probar.");
    }


    // ---------- presentacion visual ----------

    static void BeautifyScene()
    {
        // Cielo procedural azul
        var skyShader = Shader.Find("Skybox/Procedural");
        if (skyShader != null)
        {
            var sky = AssetDatabase.LoadAssetAtPath<Material>(MatDir + "/Cielo.mat");
            if (sky == null)
            {
                sky = new Material(skyShader);
                AssetDatabase.CreateAsset(sky, MatDir + "/Cielo.mat");
            }
            sky.SetFloat("_AtmosphereThickness", 0.95f);
            sky.SetFloat("_SunSize", 0.045f);
            sky.SetFloat("_Exposure", 1.15f);
            sky.SetColor("_SkyTint", new Color(0.55f, 0.75f, 0.95f));
            RenderSettings.skybox = sky;
        }

        // Luz ambiente en tres tonos + niebla suave para profundidad
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.58f, 0.74f, 0.95f);
        RenderSettings.ambientEquatorColor = new Color(0.78f, 0.74f, 0.62f);
        RenderSettings.ambientGroundColor = new Color(0.32f, 0.38f, 0.28f);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 70f;
        RenderSettings.fogEndDistance = 180f;
        RenderSettings.fogColor = new Color(0.78f, 0.88f, 0.98f);

        // Sol: sombras suaves y tono calido
        foreach (var l in Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (l.type != LightType.Directional) continue;
            l.shadows = LightShadows.Soft;
            l.intensity = 1.15f;
            l.color = new Color(1f, 0.96f, 0.88f);
            l.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }
    }

    static void StripColliders(GameObject root)
    {
        foreach (var col in root.GetComponentsInChildren<Collider>())
            Object.DestroyImmediate(col);
    }

    /// <summary>Casa inicial alrededor de los muebles: piso de madera, muros con puerta y techo de teja.</summary>
    static void BuildStarterHouse()
    {
        var shell = new GameObject("CasaInicial");
        var wallMat = GetMat("CasaMuro", new Color(0.93f, 0.9f, 0.82f));
        var floorMat = GetMat("CasaPiso", new Color(0.72f, 0.55f, 0.35f));
        var roofMat = GetMat("CasaTecho", new Color(0.78f, 0.35f, 0.3f));

        Cube("piso", shell.transform, new Vector3(0f, 0.05f, -5f), new Vector3(14f, 0.1f, 6f), floorMat);

        Cube("muroTrasero", shell.transform, new Vector3(0f, 1.3f, -8.1f), new Vector3(14f, 2.6f, 0.2f), wallMat);
        Cube("muroIzq", shell.transform, new Vector3(-7.1f, 1.3f, -5f), new Vector3(0.2f, 2.6f, 6.4f), wallMat);
        Cube("muroDer", shell.transform, new Vector3(7.1f, 1.3f, -5f), new Vector3(0.2f, 2.6f, 6.4f), wallMat);
        // Fachada con hueco de puerta al frente (z positivo)
        Cube("fachadaIzq", shell.transform, new Vector3(-4.5f, 1.3f, -1.9f), new Vector3(5f, 2.6f, 0.2f), wallMat);
        Cube("fachadaDer", shell.transform, new Vector3(4.5f, 1.3f, -1.9f), new Vector3(5f, 2.6f, 0.2f), wallMat);
        Cube("dintel", shell.transform, new Vector3(0f, 2.35f, -1.9f), new Vector3(4f, 0.5f, 0.2f), wallMat);

        Cube("techo", shell.transform, new Vector3(0f, 2.75f, -5f), new Vector3(14.6f, 0.3f, 7f), roofMat);

        // Ventanas con marco en la pared trasera
        var winMat = GetMat("Ventana", new Color(0.55f, 0.8f, 0.95f));
        var frameMat = GetMat("MarcoVentana", new Color(1f, 1f, 0.98f));
        Cube("marcoL", shell.transform, new Vector3(-4f, 1.5f, -8.16f), new Vector3(1.6f, 1.3f, 0.08f), frameMat);
        Cube("vidrioL", shell.transform, new Vector3(-4f, 1.5f, -8.17f), new Vector3(1.3f, 1.05f, 0.06f), winMat);
        Cube("marcoR", shell.transform, new Vector3(4f, 1.5f, -8.16f), new Vector3(1.6f, 1.3f, 0.08f), frameMat);
        Cube("vidrioR", shell.transform, new Vector3(4f, 1.5f, -8.17f), new Vector3(1.3f, 1.05f, 0.06f), winMat);

        // Camino de entrada (sin collider para no romper el NavMesh)
        var path = Cube("camino", shell.transform, new Vector3(0f, 0.02f, 2.5f), new Vector3(2.6f, 0.05f, 7f), GetMat("Camino", new Color(0.82f, 0.78f, 0.68f)));
        Object.DestroyImmediate(path.GetComponent<Collider>());
    }

    static void BuildTree(Vector3 pos, float scale)
    {
        var t = new GameObject("Arbol");
        t.transform.position = pos;
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "tronco";
        trunk.transform.SetParent(t.transform);
        trunk.transform.localPosition = new Vector3(0f, 0.9f * scale, 0f);
        trunk.transform.localScale = new Vector3(0.35f * scale, 0.9f * scale, 0.35f * scale);
        trunk.GetComponent<Renderer>().sharedMaterial = GetMat("Tronco", new Color(0.45f, 0.3f, 0.18f));
        var crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crown.name = "copa";
        crown.transform.SetParent(t.transform);
        crown.transform.localPosition = new Vector3(0f, 2.1f * scale, 0f);
        crown.transform.localScale = new Vector3(1.5f * scale, 1.7f * scale, 1.5f * scale);
        crown.GetComponent<Renderer>().sharedMaterial = GetMat("Hojas", new Color(0.28f, 0.58f, 0.3f));
    }

    static void BuildPalm(Transform parent, Vector3 localPos, float scale)
    {
        var p = new GameObject("Palmera");
        p.transform.SetParent(parent);
        p.transform.localPosition = localPos;
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "tronco";
        trunk.transform.SetParent(p.transform);
        trunk.transform.localPosition = new Vector3(0f, 1.9f * scale, 0f);
        trunk.transform.localScale = new Vector3(0.28f * scale, 1.9f * scale, 0.28f * scale);
        trunk.transform.localRotation = Quaternion.Euler(6f, 0f, 5f);
        trunk.GetComponent<Renderer>().sharedMaterial = GetMat("TroncoPalma", new Color(0.55f, 0.42f, 0.26f));

        var top = new GameObject("copa");
        top.transform.SetParent(p.transform);
        top.transform.localPosition = new Vector3(0.2f * scale, 3.8f * scale, 0.2f * scale);
        var leafMat = GetMat("HojaPalma", new Color(0.25f, 0.62f, 0.3f));
        for (int i = 0; i < 6; i++)
        {
            var arm = new GameObject("brazo" + i);
            arm.transform.SetParent(top.transform);
            arm.transform.localRotation = Quaternion.Euler(0f, i * 60f, 0f);
            var leaf = Cube("hoja", arm.transform, new Vector3(1.2f * scale, 0f, 0f), new Vector3(2.4f * scale, 0.1f * scale, 0.55f * scale), leafMat);
            leaf.transform.localRotation = Quaternion.Euler(0f, 0f, -14f);
        }
    }

    static void BuildCloud(Vector3 pos, float scale)
    {
        var c = new GameObject("Nube");
        c.transform.position = pos;
        var mat = GetMat("Nube", new Color(0.98f, 0.98f, 1f));
        Cube("n1", c.transform, new Vector3(0f, 0f, 0f), new Vector3(6f, 1.4f, 3.5f), mat);
        Cube("n2", c.transform, new Vector3(2.2f, 0.5f, 0.6f), new Vector3(3.5f, 1.2f, 2.6f), mat);
        Cube("n3", c.transform, new Vector3(-2f, 0.4f, -0.5f), new Vector3(3f, 1.1f, 2.4f), mat);
        c.transform.localScale = Vector3.one * scale;
        StripColliders(c);
    }

    static LocationZone MakeZone(string goName, string id, string country, LocationType type, Vector3 pos, Color color, bool startActive)
    {
        var zoneGo = new GameObject("Zona_" + goName);
        zoneGo.transform.position = pos;

        var root = new GameObject("contenido");
        root.transform.SetParent(zoneGo.transform);
        root.transform.localPosition = Vector3.zero;

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "piso";
        floor.transform.SetParent(root.transform);
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(3f, 1f, 3f);
        floor.GetComponent<Renderer>().sharedMaterial = GetMat("Zona_" + id, color);

        var spawn = new GameObject("spawn");
        spawn.transform.SetParent(root.transform);
        spawn.transform.localPosition = new Vector3(0f, 0.1f, 3f);

        var zone = zoneGo.AddComponent<LocationZone>();
        zone.info.id = id;
        zone.info.displayName = goName;
        zone.info.country = country;
        zone.info.type = type;
        zone.zoneRoot = root;
        zone.spawnPoint = spawn.transform;

        root.SetActive(startActive);
        return zone;
    }

    static void PlaceInZone(LocationZone zone, string prefabName, Vector3 localPos)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{prefabName}.prefab");
        if (prefab == null) return;
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(zone.zoneRoot.transform);
        go.transform.localPosition = localPos;
    }

    static void PlaceInScene(string prefabName, Vector3 pos)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{prefabName}.prefab");
        if (prefab == null) return;
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.position = pos;
    }

    static void BuildUI(GameManager gm, DialogueSystem dialogue)
    {
        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Barras de necesidad a la izquierda
        var needsSystem = gm.playerNeeds;
        string[] barNames = { "Bar_Hambre", "Bar_Energia", "Bar_Social", "Bar_Diversión", "Bar_Higiene" };
        var ui = canvasGo.AddComponent<NeedsUIController>();
        NeedType[] types = { NeedType.Hambre, NeedType.Energia, NeedType.Social, NeedType.Diversión, NeedType.Higiene };
        for (int i = 0; i < barNames.Length; i++)
        {
            var slider = MakeBar(canvas.transform, barNames[i], -40f + i * -30f);
            ui.bars.Add(new NeedsUIController.NeedBar { type = types[i], slider = slider, fillImage = slider.fillRect.GetComponent<Image>() });
        }

        // Botones de velocidad arriba
        string[] speedLabels = { "Pausa", "x1", "x2", "x4" };
        for (int i = 0; i < speedLabels.Length; i++)
        {
            var btn = MakeButton(canvas.transform, speedLabels[i], new Vector2(-300f + i * 80f, -20f));
            var ctrl = btn.gameObject.AddComponent<TimeControlsUI>();
            ctrl.speedIndex = i;
        }

        // Boton de sonido al lado de los controles de tiempo
        var soundBtn = MakeButton(canvas.transform, "Sonido", new Vector2(90f, -20f));
        soundBtn.gameObject.AddComponent<AudioToggleUI>();

        // Panel de misiones diarias arriba a la derecha
        var missionsGo = new GameObject("MissionsPanel", typeof(RectTransform), typeof(Text));
        var mRt = missionsGo.GetComponent<RectTransform>();
        mRt.SetParent(canvas.transform);
        mRt.anchorMin = mRt.anchorMax = new Vector2(1f, 1f);
        mRt.pivot = new Vector2(1f, 1f);
        mRt.anchoredPosition = new Vector2(-10f, -10f);
        mRt.sizeDelta = new Vector2(240f, 120f);
        var mTxt = missionsGo.GetComponent<Text>();
        mTxt.alignment = TextAnchor.UpperRight;
        mTxt.fontSize = 13;
        mTxt.color = Color.white;
        mTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        missionsGo.AddComponent<MissionsUI>();
    }

    static Slider MakeBar(Transform parent, string name, float y)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Slider));
        var rt = root.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(15f, y);
        rt.sizeDelta = new Vector2(160f, 14f);

        var bgGo = new GameObject("fondo", typeof(RectTransform), typeof(Image));
        bgGo.GetComponent<RectTransform>().SetParent(rt, false);
        Stretch(bgGo.GetComponent<RectTransform>());
        bgGo.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var fillGo = new GameObject("fill", typeof(RectTransform), typeof(Image));
        var fillRt = fillGo.GetComponent<RectTransform>();
        fillRt.SetParent(rt, false);
        Stretch(fillRt);
        fillGo.GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.35f);

        var slider = root.GetComponent<Slider>();
        slider.fillRect = fillRt;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.interactable = false;
        return slider;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Button MakeButton(Transform parent, string label, Vector2 pos)
    {
        var btnGo = new GameObject("btn_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        var rt = btnGo.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(70f, 30f);
        btnGo.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f, 0.9f);

        var txtGo = new GameObject("label", typeof(RectTransform));
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.SetParent(rt, false);
        Stretch(txtRt);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return btnGo.GetComponent<Button>();
    }
}
#endif
