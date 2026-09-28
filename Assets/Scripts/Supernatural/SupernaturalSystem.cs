using System;
using UnityEngine;

namespace MySims
{
    public enum CreatureType { Humano, Vampiro, HombreLobo, Hada, Bruja }

    /// <summary>
    /// Pack Noctambulos / Criaturas Sobrenaturales / Naturaleza Encantada: el sim
    /// puede convertirse en vampiro, hombre lobo, hada o bruja, cada uno con
    /// efectos pasivos propios que se aplican cada hora de juego.
    /// </summary>
    public class SupernaturalSystem : MonoBehaviour
    {
        public static SupernaturalSystem Instance { get; private set; }

        public CreatureType currentForm = CreatureType.Humano;
        public event Action<CreatureType> OnFormChanged;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnHourChanged += HandleHourChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HandleHourChanged; }

        public void Transform(CreatureType type)
        {
            currentForm = type;
            OnFormChanged?.Invoke(type);
            AchievementSystem.Instance?.Unlock("sobrenatural");
        }

        void HandleHourChanged(int hour)
        {
            var t = TimeSystem.Instance;
            if (t == null) return;

            switch (currentForm)
            {
                case CreatureType.Vampiro:
                    if (t.IsNight) NeedsSystem.Instance?.Recover(NeedType.Energia, 0.3f);
                    else NeedsSystem.Instance?.Recover(NeedType.Energia, -0.15f);
                    break;

                case CreatureType.HombreLobo:
                    if (t.CurrentDay % 7 == 0) SkillSystem.Instance?.GainXP(SkillType.Fitness, 5f);
                    break;

                case CreatureType.Hada:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.1f);
                    break;

                case CreatureType.Bruja:
                    SkillSystem.Instance?.GainXP(SkillType.Logica, 1f);
                    break;
            }
        }

        /// <summary>Hechizo de bruja: acelera la recuperacion de una necesidad al instante.</summary>
        public bool CastSpell(NeedType need, float amount)
        {
            if (currentForm != CreatureType.Bruja) return false;
            NeedsSystem.Instance?.Recover(need, amount);
            return true;
        }
    }
}
