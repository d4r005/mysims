using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Career
    {
        public string id = "informatica";
        public string name = "Informatica";
        public SkillType trainsSkill = SkillType.Logica;
        public float classStartHour = 8f;
        public float classEndHour = 14f;
        public float tuition = 400f;
        public float creditsToGraduate = 20f;
        public float skillXpPerClassHour = 6f;
    }

    /// <summary>
    /// Pack Universidad: el sim se inscribe en una carrera, asiste a clases en
    /// horario y junta creditos hasta graduarse. El titulo desbloquea los empleos
    /// de nivel alto del JobCatalog.
    /// </summary>
    public class UniversitySystem : MonoBehaviour
    {
        public static UniversitySystem Instance { get; private set; }

        [Header("Configuracion")]
        public Transform campusPoint;   // donde se dan las clases
        public List<Career> careers = new List<Career>
        {
            new Career { id = "informatica", name = "Informatica", trainsSkill = SkillType.Logica },
            new Career { id = "artes",      name = "Bellas Artes", trainsSkill = SkillType.Creatividad },
            new Career { id = "gastronomia", name = "Gastronomia",  trainsSkill = SkillType.Cocina },
            new Career { id = "negocios",   name = "Negocios",      trainsSkill = SkillType.Carisma }
        };

        [Header("Estado del estudiante")]
        public bool Enrolled;
        public string currentCareerId = "";
        public float credits;
        public bool Graduated;

        public bool IsClassHour { get; private set; }
        public bool IsInClass { get; private set; }

        bool commuting;
        NPCController student;

        public event Action<Career> OnEnrolled;
        public event Action<Career> OnGraduated;

        void Awake() { Instance = this; }

        void OnEnable() { TimeSystem.Instance.OnHourChanged += HandleHourChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HandleHourChanged; }

        public Career CurrentCareer =>
            careers.Find(c => c.id == currentCareerId);

        public bool TryEnroll(string careerId)
        {
            if (Enrolled || Graduated) return false;
            var career = careers.Find(c => c.id == careerId);
            if (career == null) return false;

            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(career.tuition)) return false;

            Enrolled = true;
            currentCareerId = careerId;
            credits = 0f;
            OnEnrolled?.Invoke(career);
            return true;
        }

        void HandleHourChanged(int hour)
        {
            UpdateClassHour();

            // Cobro de creditos y estudio por cada hora de clase
            if (IsInClass && IsClassHour && CurrentCareer != null)
            {
                credits++;
                SkillSystem.Instance?.GainXP(CurrentCareer.trainsSkill, CurrentCareer.skillXpPerClassHour);

                if (credits >= CurrentCareer.creditsToGraduate)
                {
                    Graduated = true;
                    Enrolled = false;
                    IsInClass = false;
                    OnGraduated?.Invoke(CurrentCareer);
                    AchievementSystem.Instance?.Unlock("graduado");
                    Debug.Log("¡Graduacion! Titulo en " + CurrentCareer.name);
                }
            }
        }

        void UpdateClassHour()
        {
            var career = CurrentCareer;
            var t = TimeSystem.Instance;
            IsClassHour = Enrolled && career != null
                && t.CurrentHour >= career.classStartHour
                && t.CurrentHour < career.classEndHour
                && (CalendarSystem.Instance == null || CalendarSystem.Instance.IsWeekend == false);
        }

        /// <summary>Llamado por NPCController en Idle despues del trabajo.</summary>
        public bool TryEngage(NPCController npc)
        {
            student = npc;
            UpdateClassHour();
            if (!Enrolled || !IsClassHour || IsInClass) return IsClassHour;

            if (!commuting)
            {
                commuting = true;
                if (campusPoint != null) npc.MoveTo(campusPoint.position);
            }
            return true;
        }

        public void Arrived(NPCController npc)
        {
            if (!commuting || npc != student) return;
            commuting = false;
            IsInClass = true;
        }

        public bool IsCommuting(NPCController npc) => commuting && npc == student;

        public void ClockOut()
        {
            IsInClass = false;
            commuting = false;
        }

        public float Progress =>
            CurrentCareer != null ? credits / CurrentCareer.creditsToGraduate : 0f;
    }
}
