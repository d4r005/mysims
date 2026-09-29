using System.IO;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Guardado local en JSON (Application.persistentDataPath funciona en Android).
    /// La nube (backend) viene en la fase 3 del roadmap.
    /// </summary>
    public static class SaveManager
    {
        static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

        public static void SaveGame()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.playerCharacter == null) return;

            var data = new SaveData
            {
                day = TimeSystem.Instance.CurrentDay,
                hour = TimeSystem.Instance.CurrentHour,
                money = EconomySystem.Instance != null ? EconomySystem.Instance.money : 0f,
                job = JobSystem.Instance != null && JobSystem.Instance.currentJob != null ? new JobData
                {
                    name = JobSystem.Instance.currentJob.name,
                    startHour = JobSystem.Instance.currentJob.startHour,
                    endHour = JobSystem.Instance.currentJob.endHour,
                    hourlyPay = JobSystem.Instance.currentJob.hourlyPay,
                    worksWeekend = JobSystem.Instance.currentJob.worksWeekend
                } : null,
                playerPosition = ToSerializable(gm.playerCharacter.transform.position),
                currentLocationId = TravelSystem.Instance != null ? TravelSystem.Instance.currentLocationId : "",
                homeLocationId = TravelSystem.Instance != null ? TravelSystem.Instance.homeLocationId : ""
            };

            if (CharacterCustomizer.Instance != null)
            {
                var ch = CharacterCustomizer.Instance;
                data.skinColor = ColorUtility.ToHtmlStringRGB(ch.skinColor);
                data.hairColor = ColorUtility.ToHtmlStringRGB(ch.hairColor);
                data.shirtColor = ColorUtility.ToHtmlStringRGB(ch.shirtColor);
                data.pantsColor = ColorUtility.ToHtmlStringRGB(ch.pantsColor);
            }

            if (AchievementSystem.Instance != null)
                data.achievements = new System.Collections.Generic.List<string>(AchievementSystem.Instance.Unlocked);

            if (UniversitySystem.Instance != null)
            {
                data.university.enrolled = UniversitySystem.Instance.Enrolled;
                data.university.graduated = UniversitySystem.Instance.Graduated;
                data.university.careerId = UniversitySystem.Instance.currentCareerId;
                data.university.credits = UniversitySystem.Instance.credits;
            }

            if (FamilySystem.Instance != null)
            {
                data.family.stage = (int)FamilySystem.Instance.stage;
                data.family.partnerName = FamilySystem.Instance.partnerName;
                foreach (var c in FamilySystem.Instance.children)
                    data.family.children.Add(new ChildData { name = c.name, ageDays = c.ageDays });
            }

            foreach (var pet in Pet.All)
                data.pets.Add(new PetData
                {
                    species = (int)pet.species,
                    petName = pet.petName,
                    carino = pet.carino,
                    hambre = pet.hambre,
                    position = ToSerializable(pet.transform.position)
                });

            if (SeasonSystem.Instance != null) { data.season = (int)SeasonSystem.Instance.currentSeason; data.weather = (int)SeasonSystem.Instance.currentWeather; }
            if (BusinessSystem.Instance != null)
            {
                data.ownsBusiness = BusinessSystem.Instance.OwnsBusiness;
                data.businessName = BusinessSystem.Instance.businessName;
                data.businessReputation = BusinessSystem.Instance.reputation;
            }
            if (FameSystem.Instance != null) { data.fameLevel = FameSystem.Instance.fameLevel; data.famePoints = FameSystem.Instance.famePoints; }
            if (SupernaturalSystem.Instance != null) data.supernaturalForm = (int)SupernaturalSystem.Instance.currentForm;
            if (EcoSystem.Instance != null) data.ecoScore = EcoSystem.Instance.ecoScore;
            if (HousingSystem.Instance != null) { data.housingType = (int)HousingSystem.Instance.housingType; data.monthlyRent = HousingSystem.Instance.monthlyRent; }
            if (FamilySystem.Instance != null) { data.familyGeneration = FamilySystem.Instance.generation; data.legacyScore = FamilySystem.Instance.legacyScore; }
            if (FutureSystem.Instance != null) { data.futureUnlocked = FutureSystem.Instance.futureUnlocked; data.timeJumps = FutureSystem.Instance.timeJumps; }

            foreach (var wall in WallPiece.All)
                data.walls.Add(new WallData
                {
                    prefabName = wall.prefabName,
                    position = ToSerializable(wall.transform.position),
                    rotationY = wall.transform.eulerAngles.y,
                    scale = ToSerializable(wall.transform.localScale)
                });

            foreach (var need in gm.playerNeeds.needs)
                data.needs.Add(new NeedData { type = (int)need.type, value = need.value });

            if (SkillSystem.Instance != null)
                foreach (var s in SkillSystem.Instance.skills)
                    data.skills.Add(new SkillData { type = (int)s.type, level = s.level, xp = s.xp });

            foreach (var obj in PlaceableObject.All)
            {
                if (!obj.placedByPlayer) continue; // los muebles de la escena no se guardan
                data.placedObjects.Add(new PlacedObjectData
                {
                    prefabName = obj.name.Replace("(Clone)", "").Trim(),
                    position = ToSerializable(obj.transform.position),
                    rotationY = obj.transform.eulerAngles.y
                });
            }

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"Partida guardada en {SavePath}");
        }

        public static bool LoadGame()
        {
            if (!File.Exists(SavePath)) return false;

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            var gm = GameManager.Instance;
            if (gm == null) return false;

            TimeSystem.Instance.StartCoroutine(RestoreNextFrame(data));
            return true;
        }

        static System.Collections.IEnumerator RestoreNextFrame(SaveData data)
        {
            yield return null; // esperar un frame a que todo este inicializado

            var gm = GameManager.Instance;
            TimeSystem.Instance.SetFromLoad(data.day, data.hour);
            gm.playerCharacter.transform.position = FromSerializable(data.playerPosition);

            if (EconomySystem.Instance != null) EconomySystem.Instance.money = data.money;
            if (JobSystem.Instance != null && data.job != null)
                JobSystem.Instance.SetJob(new Job
                {
                    name = data.job.name,
                    startHour = data.job.startHour,
                    endHour = data.job.endHour,
                    hourlyPay = data.job.hourlyPay,
                    worksWeekend = data.job.worksWeekend
                });

            if (SkillSystem.Instance != null)
                foreach (var sd in data.skills)
                {
                    var skill = SkillSystem.Instance.GetSkill((SkillType)sd.type);
                    if (skill != null) { skill.level = sd.level; skill.xp = sd.xp; }
                }

            foreach (var nd in data.needs)
            {
                var need = gm.playerNeeds.GetNeed((NeedType)nd.type);
                if (need != null) need.value = nd.value;
            }

            // Ubicacion en el mundo y casa
            if (TravelSystem.Instance != null)
            {
                TravelSystem.Instance.homeLocationId = data.homeLocationId;
                if (!string.IsNullOrEmpty(data.currentLocationId))
                    TravelSystem.Instance.TeleportTo(data.currentLocationId);
            }

            // Apariencia del personaje
            if (CharacterCustomizer.Instance != null)
            {
                var ch = CharacterCustomizer.Instance;
                ch.skinColor = ParseColor(data.skinColor, ch.skinColor);
                ch.hairColor = ParseColor(data.hairColor, ch.hairColor);
                ch.shirtColor = ParseColor(data.shirtColor, ch.shirtColor);
                ch.pantsColor = ParseColor(data.pantsColor, ch.pantsColor);
                ch.Apply();
            }

            // Universidad
            if (UniversitySystem.Instance != null)
            {
                UniversitySystem.Instance.Enrolled = data.university.enrolled;
                UniversitySystem.Instance.Graduated = data.university.graduated;
                UniversitySystem.Instance.currentCareerId = data.university.careerId;
                UniversitySystem.Instance.credits = data.university.credits;
            }

            // Familia
            if (FamilySystem.Instance != null)
            {
                FamilySystem.Instance.stage = (FamilyStage)data.family.stage;
                FamilySystem.Instance.partnerName = data.family.partnerName;
                foreach (var c in data.family.children)
                    FamilySystem.Instance.TryRestoreChild(c.name, c.ageDays);
            }

            // Mascotas
            if (PetSystem.Instance != null)
                foreach (var pd in data.pets)
                    PetSystem.Instance.RestorePet(pd);

            if (SeasonSystem.Instance != null) { SeasonSystem.Instance.currentSeason = (Season)data.season; SeasonSystem.Instance.currentWeather = (Weather)data.weather; }
            if (BusinessSystem.Instance != null)
            {
                BusinessSystem.Instance.OwnsBusiness = data.ownsBusiness;
                BusinessSystem.Instance.businessName = data.businessName;
                BusinessSystem.Instance.reputation = data.businessReputation;
            }
            if (FameSystem.Instance != null) { FameSystem.Instance.fameLevel = data.fameLevel; FameSystem.Instance.famePoints = data.famePoints; }
            if (SupernaturalSystem.Instance != null) SupernaturalSystem.Instance.currentForm = (CreatureType)data.supernaturalForm;
            if (EcoSystem.Instance != null) EcoSystem.Instance.ecoScore = data.ecoScore;
            if (HousingSystem.Instance != null) { HousingSystem.Instance.housingType = (HousingType)data.housingType; HousingSystem.Instance.monthlyRent = data.monthlyRent; }
            if (FamilySystem.Instance != null) { FamilySystem.Instance.generation = data.familyGeneration > 0 ? data.familyGeneration : 1; FamilySystem.Instance.legacyScore = data.legacyScore; }
            if (FutureSystem.Instance != null) { FutureSystem.Instance.futureUnlocked = data.futureUnlocked; FutureSystem.Instance.timeJumps = data.timeJumps; }

            // Muros construidos
            if (PrefabRegistry.Instance != null && WallBuilder.Instance != null)
                foreach (var w in data.walls)
                {
                    var prefab = PrefabRegistry.Instance.GetPrefab(w.prefabName);
                    if (prefab == null) continue;
                    var wall = Object.Instantiate(prefab, FromSerializable(w.position), Quaternion.Euler(0f, w.rotationY, 0f));
                    wall.transform.localScale = FromSerializable(w.scale);
                    var piece = wall.GetComponent<WallPiece>();
                    if (piece == null) piece = wall.AddComponent<WallPiece>();
                    piece.prefabName = w.prefabName;
                }

            // Logros
            if (AchievementSystem.Instance != null)
                AchievementSystem.Instance.RestoreUnlocked(data.achievements);

            // Restaurar muebles colocados por el jugador
            if (PrefabRegistry.Instance != null)
            {
                foreach (var po in data.placedObjects)
                {
                    var prefab = PrefabRegistry.Instance.GetPrefab(po.prefabName);
                    if (prefab == null)
                    {
                        Debug.LogWarning($"Save: prefab no encontrado en el registro: {po.prefabName}");
                        continue;
                    }
                    var placed = Object.Instantiate(prefab, FromSerializable(po.position), Quaternion.Euler(0f, po.rotationY, 0f));
                    var placeable = placed.GetComponent<PlaceableObject>();
                    if (placeable != null) placeable.placedByPlayer = true;
                }
            }
        }

        static Vector3Serializable ToSerializable(Vector3 v) =>
            new Vector3Serializable { x = v.x, y = v.y, z = v.z };

        static Vector3 FromSerializable(Vector3Serializable v) =>
            v == null ? Vector3.zero : new Vector3(v.x, v.y, v.z);

        static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrEmpty(hex)) return fallback;
            return ColorUtility.TryParseHtmlString("#" + hex, out Color c) ? c : fallback;
        }
    }
}
