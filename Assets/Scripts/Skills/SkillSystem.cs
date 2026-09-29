using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum SkillType { Cocina, Carisma, Logica, Fitness, Creatividad }

    [Serializable]
    public class Skill
    {
        public SkillType type;
        [Range(1, 10)] public int level = 1;
        public float xp;
        public float XpForNextLevel => level * 100f;

        public bool AddXp(float amount)
        {
            xp += amount;
            if (xp < XpForNextLevel) return false;
            xp -= XpForNextLevel;
            level = Mathf.Min(level + 1, 10);
            return true;
        }
    }

    /// <summary>
    /// Habilidades del personaje. Suben practicando: cocinar entrena Cocina,
    /// conversar entrena Carisma, etc. Cada nivel sube el XP requerido.
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }

        public List<Skill> skills = new List<Skill>
        {
            new Skill { type = SkillType.Cocina },
            new Skill { type = SkillType.Carisma },
            new Skill { type = SkillType.Logica },
            new Skill { type = SkillType.Fitness },
            new Skill { type = SkillType.Creatividad }
        };

        public event Action<Skill> OnSkillChanged;
        /// <summary>XP ganada en bruto (misiones diarias y audio se cuelgan aqui).</summary>
        public event Action<SkillType, float> OnXpGained;

        void Awake() { Instance = this; }

        public void GainXP(SkillType type, float amount)
        {
            var skill = GetSkill(type);
            if (skill == null) return;
            if (skill.AddXp(amount))
                Debug.Log($"¡Habilidad subida! {skill.type} nivel {skill.level}");
            OnSkillChanged?.Invoke(skill);
            OnXpGained?.Invoke(type, amount);
        }

        public Skill GetSkill(SkillType type) => skills.Find(s => s.type == type);
        public int GetLevel(SkillType type) => GetSkill(type)?.level ?? 1;
    }
}
