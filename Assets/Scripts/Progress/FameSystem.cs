using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Pack Salto a la Fama / Rumbo a la Fama: puntos de fama que suben al
    /// practicar Carisma y Creatividad. A partir de nivel 2 aparecen encuentros
    /// con fans y paparazzi (llamar RandomEncounter desde el input o rutina).
    /// </summary>
    public class FameSystem : MonoBehaviour
    {
        public static FameSystem Instance { get; private set; }

        [Range(0, 5)] public int fameLevel;
        public float famePoints;
        public float[] thresholds = { 0f, 100f, 300f, 700f, 1500f, 3000f };

        public event Action<int> OnFameLevelUp;

        void Awake() { Instance = this; }
        void OnEnable()
        {
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnSkillChanged += HandleSkillChanged;
            if (DialogueSystem.Instance != null) DialogueSystem.Instance.OnTurnPlayed += HandleTurn;
        }
        void OnDisable()
        {
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnSkillChanged -= HandleSkillChanged;
            if (DialogueSystem.Instance != null) DialogueSystem.Instance.OnTurnPlayed -= HandleTurn;
        }

        void HandleSkillChanged(Skill skill)
        {
            if (skill.type == SkillType.Creatividad || skill.type == SkillType.Carisma) AddFame(3f);
        }

        void HandleTurn(NPCRoutine npc, DialogueTopic topic) => AddFame(1f);

        public void AddFame(float amount)
        {
            famePoints += amount;
            while (fameLevel < thresholds.Length - 1 && famePoints >= thresholds[fameLevel + 1])
            {
                fameLevel++;
                OnFameLevelUp?.Invoke(fameLevel);
                if (fameLevel >= 3) AchievementSystem.Instance?.Unlock("famoso");
            }
        }

        /// <summary>Encuentro con fans o paparazzi. Llamar ocasionalmente si fameLevel >= 2.</summary>
        public void RandomEncounter()
        {
            if (fameLevel < 2) return;
            if (UnityEngine.Random.value < 0.5f)
                NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.1f);
            else
                NeedsSystem.Instance?.Recover(NeedType.Social, -0.05f);
        }
    }
}
