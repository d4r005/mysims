using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    /// <summary>
    /// Pack Hacia el Futuro: robot companero. Sigue al sim y lo ayuda con las
    /// tareas domesticas: cada hora de juego recupera un poco de la necesidad
    /// mas baja, como un sirviente robotico.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class RobotCompanion : MonoBehaviour
    {
        public static readonly List<RobotCompanion> All = new List<RobotCompanion>();

        public string robotName = "SIM-1";
        public float followDistance = 2f;
        public float helpPerHour = 3f;

        NavMeshAgent agent;

        void Awake() { agent = GetComponent<NavMeshAgent>(); }
        void OnEnable() { All.Add(this); TimeSystem.Instance.OnHourChanged += HelpOwner; }
        void OnDisable()
        {
            All.Remove(this);
            if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HelpOwner;
        }

        void Update()
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) return;
            if (Vector3.Distance(transform.position, player.transform.position) > followDistance)
                agent.SetDestination(player.transform.position);
        }

        void HelpOwner(int hour)
        {
            var needs = NeedsSystem.Instance;
            if (needs == null) return;
            var lowest = needs.GetLowestNeed();
            if (lowest != null) needs.Recover(lowest.type, helpPerHour / lowest.recoverPerHour);
            SkillSystem.Instance?.GainXP(SkillType.Logica, 1f);
        }
    }
}
