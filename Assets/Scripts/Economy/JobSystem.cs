using System;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Job
    {
        public string name = "Ayudante de cocina";
        public float startHour = 9f;
        public float endHour = 17f;
        public float hourlyPay = 15f;
        public bool worksWeekend = false;
    }

    /// <summary>
    /// Sistema de trabajos. Durante el horario laboral el personaje va al punto
    /// de trabajo y cobra por cada hora trabajada. El fin de semana descansa
    /// salvo que el trabajo lo indique.
    /// </summary>
    public class JobSystem : MonoBehaviour
    {
        public static JobSystem Instance { get; private set; }

        [Header("Configuracion")]
        public Transform workPoint;          // donde se trabaja. ej. mostrador
        public Job currentJob = new Job();

        public bool IsWorkHour { get; private set; }
        public bool IsAtWork { get; private set; }

        bool commuting;
        NPCController worker;

        public event Action<float> OnPayday; // sueldo por hora trabajada

        void Awake() { Instance = this; }

        void OnEnable()
        {
            TimeSystem.Instance.OnHourChanged += HandleHourChanged;
            TimeSystem.Instance.OnDayChanged += HandleDayChanged;
        }

        void OnDisable()
        {
            if (TimeSystem.Instance == null) return;
            TimeSystem.Instance.OnHourChanged -= HandleHourChanged;
            TimeSystem.Instance.OnDayChanged -= HandleDayChanged;
        }

        void HandleHourChanged(int hour)
        {
            UpdateWorkHour();

            // Cobra cada hora completa trabajada
            if (IsAtWork && IsWorkHour && EconomySystem.Instance != null)
            {
                EconomySystem.Instance.Add(currentJob.hourlyPay);
                OnPayday?.Invoke(currentJob.hourlyPay);
            }
        }

        void HandleDayChanged(int day)
        {
            UpdateWorkHour();
            if (!IsWorkHour) ClockOut();
        }

        void UpdateWorkHour()
        {
            var t = TimeSystem.Instance;
            IsWorkHour = currentJob != null
                && t.CurrentHour >= currentJob.startHour
                && t.CurrentHour < currentJob.endHour
                && (CalendarSystem.Instance == null || CalendarSystem.Instance.IsWeekend == false || currentJob.worksWeekend);
        }

        /// <summary>Llamado por NPCController en Idle. Devuelve true si el trabajo toma el control.</summary>
        public bool TryEngage(NPCController npc)
        {
            worker = npc;
            UpdateWorkHour();
            if (!IsWorkHour || IsAtWork) return IsWorkHour;

            if (!commuting)
            {
                commuting = true;
                if (workPoint != null) npc.MoveTo(workPoint.position);
            }
            return true; // mientras conmuta o trabaja, no persigue necesidades
        }

        /// <summary>El personaje llegó al punto de trabajo.</summary>
        public void Arrived(NPCController npc)
        {
            if (!commuting || npc != worker) return;
            commuting = false;
            IsAtWork = true;
        }

        public bool IsCommuting(NPCController npc) => commuting && npc == worker;

        public void ClockOut()
        {
            IsAtWork = false;
            commuting = false;
        }

        /// <summary>API para el menú de empleos. Validar requisitos de habilidad antes.</summary>
        public void SetJob(Job job)
        {
            currentJob = job;
            ClockOut();
        }
    }
}
