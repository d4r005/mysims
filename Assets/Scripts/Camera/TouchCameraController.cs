using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Cámara orbital para móvil:
    /// - Un dedo (drag largo): orbitar alrededor del personaje.
    /// - Dos dedos (pinch): zoom.
    /// - Dos dedos (drag): pan del objetivo.
    /// El personaje se puede desmarcar para cámara libre.
    /// </summary>
    public class TouchCameraController : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform followTarget; // normalmente el personaje

        [Header("Orbita")]
        public float distance = 10f;
        public float minDistance = 4f;
        public float maxDistance = 20f;
        public float orbitSpeed = 0.25f;
        public float panSpeed = 0.02f;
        public float smoothTime = 0.1f;

        float yaw = 45f;
        float pitch = 40f;

        // pinch
        float prevPinchDist;
        Vector3 panOffset;
        Vector3 camVelocity;

        void LateUpdate()
        {
            HandleInput();
            UpdateCameraPosition();
        }

        void HandleInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // Editor: rueda = zoom, arrastre derecho = orbitar
            distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y, minDistance, maxDistance);
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * orbitSpeed * 10f;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * orbitSpeed * 10f, 10f, 80f);
            }
#else
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved)
                {
                    yaw += t.deltaPosition.x * orbitSpeed;
                    pitch = Mathf.Clamp(pitch - t.deltaPosition.y * orbitSpeed, 10f, 80f);
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
                float pinch = Vector2.Distance(t0.position, t1.position);

                if (prevPinchDist > 0f)
                    distance = Mathf.Clamp(distance - (pinch - prevPinchDist) * 0.05f, minDistance, maxDistance);

                prevPinchDist = pinch;

                // pan: promedio del movimiento de ambos dedos
                Vector2 pan = (t0.deltaPosition + t1.deltaPosition) * 0.5f;
                panOffset -= Quaternion.Euler(0f, yaw, 0f) * new Vector3(pan.x, 0f, pan.y) * panSpeed * distance * 0.2f;
            }
            else prevPinchDist = 0f;
#endif
        }

        void UpdateCameraPosition()
        {
            if (followTarget == null) return;

            Vector3 center = followTarget.position + panOffset;
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desired = center - rot * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref camVelocity, smoothTime);
            transform.LookAt(center);
        }

        public void ResetView()
        {
            panOffset = Vector3.zero;
            yaw = 45f; pitch = 40f; distance = 10f;
        }

        /// <summary>
        /// Coloca la camara de inmediato en su posicion de orbita final, sin esperar
        /// al suavizado de Play. Util al generar la escena en el editor para que la
        /// vista previa (y el primer frame de Play) ya se vean bien encuadrados.
        /// </summary>
        public void SnapToTarget()
        {
            if (followTarget == null) return;
            Vector3 center = followTarget.position + panOffset;
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desired = center - rot * Vector3.forward * distance;
            transform.position = desired;
            camVelocity = Vector3.zero;
            transform.LookAt(center);
        }
    }
}
