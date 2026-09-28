using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Muro construido por el jugador. Se guarda con su prefab base, escala y rotación.
    /// </summary>
    public class WallPiece : MonoBehaviour
    {
        public static readonly List<WallPiece> All = new List<WallPiece>();
        public string prefabName;

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);
    }

    /// <summary>
    /// Editor de casa: dibuja paredes celda a celda. Primer tap marca el inicio,
    /// segundo tap estira la pared hasta esa celda en linea recta. Modo demoler:
    /// tap sobre una pared la elimina. Las paredes usan un prefab alargable en el eje Z.
    /// </summary>
    public class WallBuilder : MonoBehaviour
    {
        public static WallBuilder Instance { get; private set; }

        [Header("Configuracion")]
        public GameObject wallPrefab;
        public float cellSize = 1f;
        public float wallHeight = 2.5f;
        public LayerMask groundMask;
        public LayerMask wallMask;
        public float wallPrice = 20f;

        public bool BuildModeActive { get; private set; }
        public bool DemolishMode { get; private set; }

        Vector3? startPoint;
        GameObject preview;

        void Awake() { Instance = this; }

        void Update()
        {
            if (!BuildModeActive || wallPrefab == null) return;

            if (Input.GetKeyDown(KeyCode.R)) DemolishMode = !DemolishMode;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0)) HandleTap(Input.mousePosition);
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                HandleTap(Input.GetTouch(0).position);
#endif
        }

        void HandleTap(Vector2 screenPos)
        {
            if (DemolishMode) { TryDemolish(screenPos); return; }

            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask | wallMask)) return;
            if (((1 << hit.collider.gameObject.layer) & wallMask) != 0) return; // choca con muro

            Vector3 cell = Snap(hit.point);

            if (startPoint == null)
            {
                startPoint = cell;
                ShowPreview(cell);
            }
            else
            {
                BuildWall(startPoint.Value, cell);
                startPoint = null;
                if (preview != null) Destroy(preview);
            }
        }

        Vector3 Snap(Vector3 raw)
        {
            float x = Mathf.Round(raw.x / cellSize) * cellSize;
            float z = Mathf.Round(raw.z / cellSize) * cellSize;
            return new Vector3(x, 0f, z);
        }

        void ShowPreview(Vector3 from)
        {
            if (preview != null) Destroy(preview);
            preview = Instantiate(wallPrefab, from, Quaternion.identity);
            SetGhost(preview);
        }

        void SetGhost(GameObject g)
        {
            foreach (var r in g.GetComponentsInChildren<Renderer>())
            {
                var m = r.material; Color c = m.color; c.a = 0.5f; m.color = c;
            }
        }

        void BuildWall(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            delta.y = 0f;
            if (delta.magnitude < cellSize * 0.5f) return;

            // Costo por metro de muro
            var economy = EconomySystem.Instance;
            float cost = wallPrice * (delta.magnitude / cellSize);
            if (economy != null && cost > 0f && !economy.TrySpend(cost)) return;

            Vector3 center = (from + to) * 0.5f;
            float angle = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;
            var wall = Instantiate(wallPrefab, center, Quaternion.Euler(0f, angle, 0f));

            var piece = wall.GetComponent<WallPiece>();
            if (piece == null) piece = wall.AddComponent<WallPiece>();
            piece.prefabName = wallPrefab.name;

            var baseScale = wallPrefab.transform.localScale;
            wall.transform.localScale = new Vector3(baseScale.x, wallHeight, delta.magnitude);

            AchievementSystem.Instance?.NotifyWallBuilt();
        }

        void TryDemolish(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, wallMask)) return;

            var piece = hit.collider.GetComponentInParent<WallPiece>();
            if (piece != null)
            {
                Destroy(piece.gameObject);
                EconomySystem.Instance?.Add(wallPrice * 0.5f); // reembolso parcial
            }
        }

        public void EnterWallMode(GameObject prefab)
        {
            wallPrefab = prefab;
            BuildModeActive = true;
            DemolishMode = false;
            startPoint = null;
        }

        public void ExitWallMode()
        {
            BuildModeActive = false;
            startPoint = null;
            if (preview != null) Destroy(preview);
        }
    }
}
