using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Ghost { public string name; public string causeOfDeath; }

    /// <summary>
    /// Pack Vida y Mas Alla: modo opcional de permadeath. Si una necesidad llega
    /// a 0 por mucho tiempo hay una probabilidad baja de muerte; el sim se vuelve
    /// fantasma y puede revivir con un objeto/ritual especial.
    /// </summary>
    public class AfterlifeSystem : MonoBehaviour
    {
        public static AfterlifeSystem Instance { get; private set; }

        public bool permadeathEnabled = false;
        public bool IsGhost { get; private set; }
        public List<Ghost> familyGhosts = new List<Ghost>();

        public event Action<Ghost> OnDeath;
        public event Action OnResurrected;

        void Awake() { Instance = this; }
        void OnEnable() { if (permadeathEnabled) TimeSystem.Instance.OnHourChanged += CheckDeath; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= CheckDeath; }

        void CheckDeath(int hour)
        {
            if (IsGhost || NeedsSystem.Instance == null) return;
            var lowest = NeedsSystem.Instance.GetLowestNeed();
            if (lowest.value <= 0f && UnityEngine.Random.value < 0.02f) Die(lowest.type.ToString());
        }

        public void Die(string cause)
        {
            IsGhost = true;
            var ghost = new Ghost { name = "Sim", causeOfDeath = cause };
            familyGhosts.Add(ghost);
            OnDeath?.Invoke(ghost);
            AchievementSystem.Instance?.Unlock("mas_alla");
        }

        public void Resurrect()
        {
            if (!IsGhost) return;
            IsGhost = false;
            foreach (var need in NeedsSystem.Instance.needs) need.value = 80f;
            OnResurrected?.Invoke();
        }
    }
}
