using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    /// <summary>
    /// Animación procedural básica (sin rigs ni FBX):
    /// - Bob al caminar (rebote suave acorde a velocidad).
    /// - Inclinación hacia la dirección de movimiento.
    /// - Rotación suave hacia donde camina.
    /// - Parpadeo/breathing sutil en idle.
    /// Cuando haya un modelo con Animator, este script se puede reemplazar
    /// por un AnimatorController (walk/idle/sit); la interfaz queda igual.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimpleLocomotion : MonoBehaviour
    {
        [Header("Bob al caminar")]
        public float bobFrequency = 10f;
        public float bobAmplitude = 0.06f;
        public float leanAngle = 8f;
        public float turnSpeed = 12f;

        [Header("Idle")]
        public float breatheAmplitude = 0.01f;
        public float breatheFrequency = 1.5f;

        NavMeshAgent agent;
        Vector3 baseLocalPos;
        float bobTimer;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            baseLocalPos = transform.localPosition;
        }

        void Update()
        {
            Vector3 vel = agent.velocity;
            float speed = vel.magnitude;
            bool moving = speed > 0.1f;

            // Bob + lean al caminar, respiración en idle
            if (moving)
            {
                bobTimer += Time.deltaTime * bobFrequency * (speed / agent.speed);
                float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitude;
                transform.localPosition = baseLocalPos + Vector3.up * bob;

                // Inclinación hacia delante proporcional a la velocidad
                float lean = leanAngle * (speed / Mathf.Max(agent.speed, 0.1f));
                transform.localRotation = Quaternion.Euler(lean, transform.localRotation.eulerAngles.y, 0f);

                // Girar suave hacia la dirección de movimiento
                if (vel.sqrMagnitude > 0.01f)
                {
                    Quaternion target = Quaternion.LookRotation(vel.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
                }
            }
            else
            {
                float breathe = Mathf.Sin(Time.time * breatheFrequency) * breatheAmplitude;
                transform.localPosition = baseLocalPos + Vector3.up * breathe;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, transform.localRotation.eulerAngles.y, 0f), 5f * Time.deltaTime);
            }
        }
    }
}
