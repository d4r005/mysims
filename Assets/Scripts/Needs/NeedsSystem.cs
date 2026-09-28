using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Need
    {
        public NeedType type;
        [Range(0f, 100f)] public float value = 80f;
        [Tooltip("Puntos de necesidad que se pierden por hora de juego")]
        public float decayPerHour = 4f;
        [Tooltip("Puntos por hora que recupera al usar el objeto adecuado")]
        public float recoverPerHour = 40f;

        public bool IsCritical => value <= 20f;
    }

    /// <summary>
    /// Sistema de necesidades del personaje. Cada hora de juego las necesidades bajan;
    /// al usar un objeto (cama, refri, ducha...) la necesidad asociada se recupera.
    /// </summary>
    public class NeedsSystem : MonoBehaviour
    {
        public static NeedsSystem Instance { get; private set; }

        public List<Need> needs = new List<Need>
        {
            new Need { type = NeedType.Hambre,     decayPerHour = 5f,  recoverPerHour = 60f },
            new Need { type = NeedType.Energia,   decayPerHour = 4f,  recoverPerHour = 30f },
            new Need { type = NeedType.Social,   decayPerHour = 3f,  recoverPerHour = 25f },
            new Need { type = NeedType.Diversión, decayPerHour = 3f,  recoverPerHour = 35f },
            new Need { type = NeedType.Higiene,  decayPerHour = 2f,  recoverPerHour = 80f }
        };

        public event Action<Need> OnNeedChanged;

        void Awake() { Instance = this; }

        void OnEnable()
        {
            if (TimeSystem.Instance != null)
                TimeSystem.Instance.OnHourChanged += HandleHourChanged;
        }

        void OnDisable()
        {
            if (TimeSystem.Instance != null)
                TimeSystem.Instance.OnHourChanged -= HandleHourChanged;
        }

        void HandleHourChanged(int hour)
        {
            foreach (var need in needs)
            {
                need.value = Mathf.Max(0f, need.value - need.decayPerHour);
                OnNeedChanged?.Invoke(need);
            }
        }

        /// <summary>Recupera una necesidad mientras se usa un objeto (llamar cada frame de uso).</summary>
        public void Recover(NeedType type, float amountPerHourScale)
        {
            var need = GetNeed(type);
            if (need == null) return;
            need.value = Mathf.Min(100f, need.value + need.recoverPerHour * amountPerHourScale);
            OnNeedChanged?.Invoke(need);
        }

        public Need GetNeed(NeedType type) => needs.Find(n => n.type == type);
        public Need GetLowestNeed() => needs.OrderBy(n => n.value).First();
    }
}
