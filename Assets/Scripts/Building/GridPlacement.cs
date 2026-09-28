using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Modo construccion: coloca el objeto seleccionado en una cuadricula con snap.
    /// Uso: asignar este script a la camara, un prefab a "objectToPlace".
    /// Tap/click en el suelo coloca; R rota 90 grados.
    /// </summary>
    public class GridPlacement : MonoBehaviour
    {
        [Header("Configuracion")]
        public GameObject objectToPlace;
        public LayerMask groundMask;          // capa del suelo
        public LayerMask blockingMask;        // capas que impiden colocar (muros, otros muebles)
        public float cellSize = 1f;
        public float rotationStep = 90f;

        public bool BuildModeActive { get; private set; }

        GameObject ghost;  // vista previa fantasma

        void Update()
        {
            if (!BuildModeActive || objectToPlace == null) return;

            if (Input.GetKeyDown(KeyCode.R)) RotateGhost();
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                if (TryGetPlacementPoint(out Vector3 point))
                {
                    PlaceObject(point);
                }
            }
        }

        bool TryGetPlacementPoint(out Vector3 point)
        {
            point = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask | blockingMask)) return false;

            if (((1 << hit.collider.gameObject.layer) & blockingMask) != 0) return false; // ocupado

            point = SnapToGrid(hit.point);
            return true;
        }

        Vector3 SnapToGrid(Vector3 raw)
        {
            float x = Mathf.Round(raw.x / cellSize) * cellSize;
            float z = Mathf.Round(raw.z / cellSize) * cellSize;
            return new Vector3(x, 0f, z);
        }

        void RotateGhost()
        {
            if (ghost != null) ghost.transform.Rotate(0f, rotationStep, 0f);
        }

        void PlaceObject(Vector3 point)
        {
            Instantiate(objectToPlace, point, ghost != null ? ghost.transform.rotation : Quaternion.identity);
        }

        public void EnterBuildMode(GameObject prefab)
        {
            objectToPlace = prefab;
            BuildModeActive = true;
            if (ghost != null) Destroy(ghost);
            if (prefab != null)
            {
                ghost = Instantiate(prefab);
                SetGhostTransparency();
            }
        }

        public void ExitBuildMode()
        {
            BuildModeActive = false;
            if (ghost != null) Destroy(ghost);
        }

        void SetGhostTransparency()
        {
            foreach (var r in ghost.GetComponentsInChildren<Renderer>())
            {
                var mat = r.material;
                Color c = mat.color; c.a = 0.5f;
                mat.color = c;
            }
        }
    }
}
