using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Pack Anos High School: version adolescente de la universidad. Asistir a
    /// clases sube la calificacion; graduarse habilita la inscripcion universitaria.
    /// </summary>
    public class HighSchoolSystem : MonoBehaviour
    {
        public static HighSchoolSystem Instance { get; private set; }

        public Transform schoolPoint;
        public float classStartHour = 7f, classEndHour = 13f;
        public bool Enrolled = true;
        public bool Graduated;
        [Range(0, 100)] public float grade = 70f;

        bool commuting, inClass;
        NPCController student;

        public event Action OnGraduated;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnHourChanged += HandleHourChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HandleHourChanged; }

        void HandleHourChanged(int hour)
        {
            if (!Enrolled || Graduated) return;
            var t = TimeSystem.Instance;
            bool isClassHour = t.CurrentHour >= classStartHour && t.CurrentHour < classEndHour
                && (CalendarSystem.Instance == null || !CalendarSystem.Instance.IsWeekend);

            if (inClass && isClassHour) grade = Mathf.Min(100f, grade + 2f);
            if (!isClassHour) inClass = false;
        }

        public bool TryEngage(NPCController npc)
        {
            student = npc;
            var t = TimeSystem.Instance;
            bool isClassHour = Enrolled && !Graduated && t.CurrentHour >= classStartHour && t.CurrentHour < classEndHour;
            if (!isClassHour) return false;

            if (!commuting && !inClass) { commuting = true; if (schoolPoint != null) npc.MoveTo(schoolPoint.position); }
            return true;
        }

        public void Arrived(NPCController npc)
        {
            if (!commuting || npc != student) return;
            commuting = false; inClass = true;
        }

        public bool IsCommuting(NPCController npc) => commuting && npc == student;
        public bool IsInClass => inClass;
        public void ClockOut() { commuting = false; inClass = false; }

        public void Graduate()
        {
            if (grade < 60f) return;
            Graduated = true;
            Enrolled = false;
            OnGraduated?.Invoke();
            AchievementSystem.Instance?.Unlock("bachiller");
        }
    }
}
