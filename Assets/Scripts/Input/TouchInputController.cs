using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Entrada táctil para Android:
    /// - Tap corto en el suelo: el personaje camina a ese punto.
    /// - Tap sobre un PlaceableObject: camina a su interactionPoint y lo usa.
    /// Funciona igual con mouse en el editor (para probar sin celular).
    /// </summary>
    public class TouchInputController : MonoBehaviour
    {
        [Header("Referencias")]
        public NPCController playerCharacter;
        public LayerMask groundMask;
        public LayerMask objectMask;

        [Header("Configuracion")]
        public float tapMaxDuration = 0.25f;   // segundos (mas de esto = drag, no tap)
        public float tapMaxMovement = 40f;     // pixels de tolerancia

        Vector2 startPos;
        float startTime;
        int tapFingerId = -1;

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0)) { startPos = Input.mousePosition; startTime = Time.time; }
            if (Input.GetMouseButtonUp(0))
            {
                if (Vector2.Distance(Input.mousePosition, startPos) > tapMaxMovement) return;
                if (Time.time - startTime > tapMaxDuration) return;
                ProcessTap(Input.mousePosition);
            }
        }

        void HandleTouch()
        {
            if (Input.touchCount != 1) { tapFingerId = -1; return; }

            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) { startPos = t.position; startTime = Time.time; tapFingerId = t.fingerId; }
            else if (t.phase == TouchPhase.Ended && t.fingerId == tapFingerId)
            {
                bool isTap = Time.time - startTime <= tapMaxDuration
                          && Vector2.Distance(t.position, startPos) <= tapMaxMovement;
                if (isTap) ProcessTap(t.position);
                tapFingerId = -1;
            }
        }

        void ProcessTap(Vector2 screenPos)
        {
            if (playerCharacter == null) return;
            if (GridPlacement.Instance != null && GridPlacement.Instance.BuildModeActive) return; // el modo construccion maneja sus taps

            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // 1. ¿tap en un objeto interactivo?
            if (Physics.Raycast(ray, out RaycastHit objHit, 100f, objectMask))
            {
                var placeable = objHit.collider.GetComponentInParent<PlaceableObject>();
                if (placeable != null)
                {
                    playerCharacter.MoveTo(placeable.interactionPoint.position);
                    return;
                }
            }

            // 2. ¿tap en el suelo?
            if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, groundMask))
            {
                playerCharacter.MoveTo(groundHit.point);
            }
        }
    }
}
