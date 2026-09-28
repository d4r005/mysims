using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class Employee { public string npcName; public float wage = 12f; }

    /// <summary>
    /// Pack Abren Negocios / Ocio y Negocio: el jugador funda un negocio propio
    /// (tienda, restaurante, taller...), contrata vecinos como empleados y cobra
    /// ingresos por hora segun su reputacion.
    /// </summary>
    public class BusinessSystem : MonoBehaviour
    {
        public static BusinessSystem Instance { get; private set; }

        [Header("Estado")]
        public bool OwnsBusiness;
        public string businessName = "";
        [Range(0, 100)] public float reputation = 50f;
        public List<Employee> employees = new List<Employee>();

        public float openingCost = 400f;
        public float hourlyIncomeBase = 20f;
        public float startHour = 10f, endHour = 20f;

        public event Action<float> OnIncome;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnHourChanged += HandleHourChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= HandleHourChanged; }

        public bool TryOpenBusiness(string name)
        {
            if (OwnsBusiness) return false;
            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(openingCost)) return false;
            OwnsBusiness = true;
            businessName = name;
            AchievementSystem.Instance?.Unlock("emprendedor");
            return true;
        }

        public bool Hire(string npcName, float wage)
        {
            if (!OwnsBusiness) return false;
            employees.Add(new Employee { npcName = npcName, wage = wage });
            return true;
        }

        void HandleHourChanged(int hour)
        {
            if (!OwnsBusiness) return;
            var t = TimeSystem.Instance;
            bool open = t.CurrentHour >= startHour && t.CurrentHour < endHour;
            if (!open) return;

            float staffBoost = 1f + employees.Count * 0.15f;
            float income = hourlyIncomeBase * (reputation / 50f) * staffBoost;
            float wages = 0f;
            foreach (var e in employees) wages += e.wage;

            float net = Mathf.Max(0f, income - wages);
            EconomySystem.Instance?.Add(net);
            OnIncome?.Invoke(net);
            reputation = Mathf.Clamp(reputation + UnityEngine.Random.Range(-1f, 1.5f), 0f, 100f);
        }
    }
}
