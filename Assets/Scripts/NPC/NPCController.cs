using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    public enum NPCState { Idle, Walking, UsingObject, Working }

    /// <summary>
    /// Controlador del personaje (jugador o NPC).
    /// FSM simple: cuando una necesidad baja, busca el objeto mas cercano
    /// que la satisface, camina hasta el y lo usa hasta recuperarse.
    /// Requiere: NavMeshAgent + NavMesh en la escena.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : MonoBehaviour
    {
        [Header("Configuracion")]
        public bool isPlayerControlled = true;
        public float minNeedThreshold = 45f; // debajo de esto, actuar
        public float satisfiedThreshold = 95f;

        public NPCState State { get; private set; } = NPCState.Idle;

        NavMeshAgent agent;
        PlaceableObject currentTarget;
        float needCheckTimer;
        const float NEED_CHECK_INTERVAL = 1f;

        void Awake() { agent = GetComponent<NavMeshAgent>(); }

        void Update()
        {
            needCheckTimer += Time.deltaTime;
            if (needCheckTimer < NEED_CHECK_INTERVAL) return;
            needCheckTimer = 0f;

            switch (State)
            {
                case NPCState.Idle:
                    HandleIdle();
                    break;

                case NPCState.Walking:
                    HandleWalking();
                    break;

                case NPCState.UsingObject:
                    HandleUsing();
                    break;

                case NPCState.Working:
                    HandleWorking();
                    break;
            }
        }

        void HandleIdle()
        {
            // El trabajo tiene prioridad sobre las necesidades
            if (JobSystem.Instance != null && JobSystem.Instance.TryEngage(this))
            {
                State = JobSystem.Instance.IsAtWork ? NPCState.Working : NPCState.Walking;
                return;
            }

            if (NeedsSystem.Instance == null) return;
            var need = NeedsSystem.Instance.GetLowestNeed();
            if (need == null || need.value > minNeedThreshold) return;

            var obj = PlaceableObject.FindClosestForNeed(need.type, transform.position);
            if (obj == null) return; // no hay objeto que la satisfaga

            currentTarget = obj;
            agent.SetDestination(obj.interactionPoint.position);
            State = NPCState.Walking;
        }

        void HandleWalking()
        {
            if (currentTarget == null) { State = NPCState.Idle; return; }

            if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) return;

            // ¿Llegó al trabajo?
            if (JobSystem.Instance != null && JobSystem.Instance.IsCommuting(this))
            {
                JobSystem.Instance.Arrived(this);
                State = NPCState.Working;
                return;
            }

            if (currentTarget != null)
            {
                currentTarget.StartUse(this);
                State = NPCState.UsingObject;
            }
            else State = NPCState.Idle;
        }

        void HandleWorking()
        {
            if (JobSystem.Instance == null || !JobSystem.Instance.IsWorkHour)
            {
                JobSystem.Instance?.ClockOut();
                State = NPCState.Idle;
            }
        }

        void HandleUsing()
        {
            if (currentTarget == null) { State = NPCState.Idle; return; }

            // Mientras usa el objeto, la necesidad sube (la recupera TimeSystem por hora)
            currentTarget.ContinueUse(this);

            // Practicar la habilidad que entrena el objeto mientras lo usa
            if (currentTarget.trainsSkill != SkillType.Carisma && currentTarget.skillXpPerUse > 0f)
                SkillSystem.Instance?.GainXP(currentTarget.trainsSkill, currentTarget.skillXpPerUse * NEED_CHECK_INTERVAL);

            var need = NeedsSystem.Instance.GetNeed(currentTarget.satisfies);
            if (need == null || need.value >= satisfiedThreshold)
            {
                currentTarget.StopUse(this);
                currentTarget = null;
                State = NPCState.Idle;
            }
        }

        /// <summary>El jugador ordena ir a un punto (tap en pantalla).</summary>
        public void MoveTo(Vector3 destination)
        {
            currentTarget?.StopUse(this);
            currentTarget = null;
            agent.SetDestination(destination);
            State = NPCState.Walking;
        }
    }
}
