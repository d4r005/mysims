using UnityEngine;

namespace MySims
{
    /// <summary>
    /// El plumbob de Los Sims: el diamante verde que flota y gira sobre la cabeza
    /// del sim. Se construye por código (no necesita prefab ni assets). Si el
    /// personaje tiene NeedsSystem, el color refleja su ánimo promedio:
    /// verde (bien) -> amarillo -> naranja -> rojo (necesidades críticas).
    /// Montaje: AddComponent en la raíz del personaje.
    /// </summary>
    public class Plumbob : MonoBehaviour
    {
        [Header("Configuración")]
        public float height = 2.3f;
        public float size = 0.38f;
        public float spinSpeed = 140f;
        public float bobAmount = 0.05f;
        public bool followMood = true;

        static readonly Color MoodGood = new Color32(0x33, 0xE0, 0x35, 0xFF);
        static readonly Color MoodMedium = new Color(0.98f, 0.85f, 0.12f);
        static readonly Color MoodLow = new Color(1f, 0.45f, 0.05f);
        static readonly Color MoodCritical = new Color(0.85f, 0.12f, 0.08f);

        static Mesh diamondMesh;
        static Material diamondMat;

        NeedsSystem needs;
        Transform root;
        Transform spinner;
        Color current = MoodGood;
        float t;

        void Start()
        {
            if (followMood) needs = GetComponentInParent<NeedsSystem>();

            if (diamondMesh == null) diamondMesh = BuildDiamondMesh();
            if (diamondMat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Color");
                if (shader == null) shader = Shader.Find("Standard");
                diamondMat = new Material(shader);
                ApplyColor(diamondMat, MoodGood);
            }

            root = new GameObject("Plumbob").transform;
            root.SetParent(transform, false);
            root.localPosition = new Vector3(0f, height, 0f);

            spinner = new GameObject("diamante").transform;
            spinner.SetParent(root, false);
            spinner.localScale = Vector3.one * size;

            var meshGo = new GameObject("malla", typeof(MeshFilter), typeof(MeshRenderer));
            meshGo.transform.SetParent(spinner, false);
            meshGo.GetComponent<MeshFilter>().sharedMesh = diamondMesh;
            var rend = meshGo.GetComponent<MeshRenderer>();
            rend.sharedMaterial = diamondMat;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            rend.receiveShadows = false;
        }

        void Update()
        {
            if (root == null) return;
            t += Time.deltaTime;

            // Flota suave y gira siempre, como el original
            root.localPosition = new Vector3(0f, height + Mathf.Sin(t * 2.2f) * bobAmount, 0f);
            if (spinner != null) spinner.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);

            if (!followMood || needs == null) return;

            // Animo promedio de las 5 necesidades -> color del diamante
            float avg = 0f;
            foreach (var n in needs.needs) avg += n.value;
            avg /= Mathf.Max(1, needs.needs.Count);

            var target = avg >= 65f ? MoodGood
                : avg >= 40f ? MoodMedium
                : avg >= 20f ? MoodLow : MoodCritical;
            current = Color.Lerp(current, target, Time.deltaTime * 3f);
            ApplyColor(diamondMat, current);
        }

        static void ApplyColor(Material mat, Color c)
        {
            if (mat == null) return;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", c * 0.6f);
            }
        }

        /// <summary>Octaedro alargado: punta alta hacia arriba, corta hacia abajo, como el plumbob real.</summary>
        static Mesh BuildDiamondMesh()
        {
            var top = new Vector3(0f, 1f, 0f);
            var bottom = new Vector3(0f, -0.55f, 0f);
            const float ry = 0.1f, rx = 0.55f;
            var r0 = new Vector3(rx, ry, 0f);
            var r1 = new Vector3(0f, ry, rx);
            var r2 = new Vector3(-rx, ry, 0f);
            var r3 = new Vector3(0f, ry, -rx);

            var verts = new[] { top, bottom, r0, r1, r2, r3 };
            int[][] faces =
            {
                new[]{0,3,2}, new[]{0,4,3}, new[]{0,5,4}, new[]{0,2,5}, // pirámide superior
                new[]{1,2,3}, new[]{1,3,4}, new[]{1,4,5}, new[]{1,5,2}, // pirámide inferior
            };

            // Cada cara se agrega con ambos sentidos de giro, así el diamante se ve
            // bien sin depender del winding del render pipeline que use el proyecto.
            var tris = new int[faces.Length * 6];
            int k = 0;
            foreach (var f in faces)
            {
                tris[k++] = f[0]; tris[k++] = f[1]; tris[k++] = f[2];
                tris[k++] = f[0]; tris[k++] = f[2]; tris[k++] = f[1];
            }

            var mesh = new Mesh { name = "Plumbob" };
            mesh.vertices = verts;
            mesh.triangles = tris;
            return mesh;
        }
    }
}
