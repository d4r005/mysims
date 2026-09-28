using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    [Serializable]
    public class RoutineEntry
    {
        [Tooltip("Hora del dia en que empieza esta actividad")]
        public float startHour;
        [Tooltip("Que hace a esta hora")]
        public RoutineActivity activity;
        [Tooltip("Radio de deambular para Wander")]
        public float wanderRadius = 5f;
    }

    public enum RoutineActivity
    {
        Deambular,      // pasear por la zona
        UsarObjeto,     // usar un mueble aleatorio
        Dormir,         // quedarse quieto hasta la proxima entrada
        Conversar       // buscar a otro personaje
    }

    /// <summary>
    /// Rutina diaria de un NPC secundario: a cada hora elige la actividad activa
    /// segun su horario. Colocar en el mismo GameObject que el NPCController del NPC.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCRoutine : MonoBehaviour
    {
        [Header("Identidad")]
        public string npcName = "Vecino";

        [Header("Rutina del dia. La entrada con la mayor hora menor o igual a la actual manda.")]
        public List<RoutineEntry> schedule = new List<RoutineEntry>
        {
            new RoutineEntry { startHour = 8f,  activity = RoutineActivity.Deambular },
            new RoutineEntry { startHour = 14f, activity = RoutineActivity.UsarObjeto },
            new RoutineEntry { startHour = 22f, activity = RoutineActivity.Dormir }
        };

        public bool IsBusy { get; set; }   // en conversacion
        public Transform Transform => transform;

        NavMeshAgent agent;

        void Awake() { agent = GetComponent<NavMeshAgent>(); }

        void OnEnable() { TimeSystem.Instance.OnHourChanged += HandleHourChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HandleHourChanged; }

        void HandleHourChanged(int hour)
        {
            if (IsBusy) return;
            var entry = ActiveEntry();
            if (entry == null) return;
            Execute(entry);
        }

        RoutineEntry ActiveEntry()
        {
            float h = TimeSystem.Instance.CurrentHour;
            RoutineEntry best = null;
            foreach (var e in schedule)
                if (e.startHour <= h && (best == null || e.startHour > best.startHour))
                    best = e;
            return best;
        }

        void Execute(RoutineEntry entry)
        {
            switch (entry.activity)
            {
                case RoutineActivity.Deambular:
                    Vector3 target = transform.position + new Vector3(
                        UnityEngine.Random.Range(-1f, 1f) * entry.wanderRadius, 0f,
                        UnityEngine.Random.Range(-1f, 1f) * entry.wanderRadius);
                    agent.SetDestination(target);
                    break;

                case RoutineActivity.UsarObjeto:
                    if (PlaceableObject.All.Count > 0)
                    {
                        var obj = PlaceableObject.All[UnityEngine.Random.Range(0, PlaceableObject.All.Count)];
                        if (obj.interactionPoint != null)
                            agent.SetDestination(obj.interactionPoint.position);
                    }
                    break;

                case RoutineActivity.Dormir:
                    agent.ResetPath(); // se queda donde esta
                    break;

                case RoutineActivity.Conversar:
                    DialogueSystem.Instance?.TryStartConversationWith(this);
                    break;
            }
        }
    }
}
