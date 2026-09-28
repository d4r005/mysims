using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
    }

    /// <summary>
    /// Logros del juego. Escucha los eventos de los sistemas y desbloquea cuando
    /// se cumplen las condiciones. La UI puede engancharse a OnUnlocked para toasts.
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        public static AchievementSystem Instance { get; private set; }

        [Header("Definiciones")]
        public List<Achievement> definitions = new List<Achievement>
        {
            new Achievement { id = "primer_sueldo",  title = "Primer sueldo",     description = "Cobra tu primera hora de trabajo." },
            new Achievement { id = "ahorrador",     title = "Ahorrador",          description = "Acumula $1,000 o mas." },
            new Achievement { id = "vida_social",   title = "Vida social",         description = "Alcanza relacion 50 o mas con un vecino." },
            new Achievement { id = "experto",        title = "Experto",            description = "Sube cualquier habilidad a nivel 5." },
            new Achievement { id = "mochilero",     title = "Mochilero",          description = "Visita 3 lugares del mundo." },
            new Achievement { id = "constructor",   title = "Constructor",        description = "Construye tu primera pared." },
            new Achievement { id = "graduado",      title = "Graduado",            description = "Termina una carrera universitaria." },
            new Achievement { id = "boda",          title = "La gran boda",         description = "Casa-te con tu pareja." },
            new Achievement { id = "familia",       title = "Familia numerosa",    description = "Ten 2 hijos o mas." },
            new Achievement { id = "mascota",       title = "Mejor amigo peludo",  description = "Lleva el carino de tu mascota a 80 o mas." }
        };

        public event Action<Achievement> OnUnlocked;

        void Awake() { Instance = this; }

        void OnEnable()
        {
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged += TrackMoney;
            if (JobSystem.Instance != null) JobSystem.Instance.OnPayday += TrackWork;
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnSkillChanged += TrackSkill;
            if (DialogueSystem.Instance != null) DialogueSystem.Instance.OnTurnPlayed += TrackRelation;
        }

        void OnDisable()
        {
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged -= TrackMoney;
            if (JobSystem.Instance != null) JobSystem.Instance.OnPayday -= TrackWork;
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnSkillChanged -= TrackSkill;
            if (DialogueSystem.Instance != null) DialogueSystem.Instance.OnTurnPlayed -= TrackRelation;
        }

        void TrackMoney(float money)
        {
            if (money >= 1000f) Unlock("ahorrador");
        }

        void TrackWork(float pay) { Unlock("primer_sueldo"); }

        void TrackSkill(Skill skill)
        {
            if (skill.level >= 5) Unlock("experto");
        }

        void TrackRelation(NPCRoutine npc, DialogueTopic topic)
        {
            if (DialogueSystem.Instance != null && DialogueSystem.Instance.GetRelation(npc) >= 50f)
                Unlock("vida_social");
        }

        public void NotifyWallBuilt() { Unlock("constructor"); }
        public void NotifyTraveled() { UnlockCheckTravel(); }

        void UnlockCheckTravel()
        {
            if (TravelSystem.Instance != null && TravelSystem.Instance.locationsVisitedCount >= 3)
                Unlock("mochilero");
        }

        public HashSet<string> Unlocked { get; } = new HashSet<string>();

        public void Unlock(string id)
        {
            if (Unlocked.Contains(id)) return;
            if (definitions.Find(d => d.id == id) == null) return;

            Unlocked.Add(id);
            var def = definitions.Find(d => d.id == id);
            Debug.Log($"Logro desbloqueado: {def.title}");
            OnUnlocked?.Invoke(def);
        }

        public void RestoreUnlocked(List<string> ids)
        {
            Unlocked.Clear();
            foreach (var id in ids) Unlock(id);
        }
    }
}
