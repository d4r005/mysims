using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum MissionKind { UseObjects, SkillXp, EarnMoney, Travel }

    [Serializable]
    public class DailyMission
    {
        public MissionKind kind;
        public string title;
        public string detail;
        public float progress;
        public float target;
        public float reward;
        public bool done;
    }

    /// <summary>
    /// Misiones diarias: 3 al dia generadas con semilla del dia de juego.
    /// Se completan jugando (usar muebles, entrenar habilidad, ganar dinero, viajar)
    /// y pagan recompensa en efectivo. Al completar 5 misiones en total se
    /// desbloquea el logro "misionero".
    /// </summary>
    public class DailyMissionSystem : MonoBehaviour
    {
        public static DailyMissionSystem Instance { get; private set; }

        public List<DailyMission> missions = new List<DailyMission>();
        public int totalCompleted;
        public event Action OnMissionsChanged;

        float lastMoney;
        bool moneyInitialized;

        void Awake() { Instance = this; }

        void Start()
        {
            if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged += OnNewDay;
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged += OnMoneyChanged;
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnXpGained += OnXpGained;
            if (TravelSystem.Instance != null) TravelSystem.Instance.OnTravel += OnTravel;
            PlaceableObject.OnUsed += OnObjectUsed;
            OnNewDay(TimeSystem.Instance != null ? TimeSystem.Instance.CurrentDay : 1);
        }

        void OnDestroy()
        {
            if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= OnNewDay;
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged -= OnMoneyChanged;
            if (SkillSystem.Instance != null) SkillSystem.Instance.OnXpGained -= OnXpGained;
            if (TravelSystem.Instance != null) TravelSystem.Instance.OnTravel -= OnTravel;
            PlaceableObject.OnUsed -= OnObjectUsed;
        }

        // ---------- generacion diaria ----------

        void OnNewDay(int day)
        {
            var rng = new System.Random(day * 7919);
            missions.Clear();

            var pool = new List<Action<System.Random>>
            {
                r => { int n = 2 + r.Next(3); missions.Add(new DailyMission {
                    kind = MissionKind.UseObjects, title = "Vida activa",
                    detail = $"Usa {n} muebles", target = n, reward = 60f + r.Next(4) * 20f }); },
                r => {
                    var skill = (SkillType)r.Next(5);
                    int xp = 10 + r.Next(3) * 5;
                    missions.Add(new DailyMission {
                        kind = MissionKind.SkillXp, title = "Practica",
                        detail = $"Gana {xp} XP de {skill}", target = xp,
                        reward = 80f + r.Next(4) * 20f });
                },
                r => { int amount = 150 + r.Next(4) * 50; missions.Add(new DailyMission {
                    kind = MissionKind.EarnMoney, title = "Ahorro",
                    detail = $"Gana ${amount}", target = amount, reward = 100f + r.Next(3) * 50f }); },
                r => { int n = 1 + r.Next(2); missions.Add(new DailyMission {
                    kind = MissionKind.Travel, title = "Turista",
                    detail = $"Realiza {n} viaje(s)", target = n, reward = 120f + r.Next(3) * 40f }); },
            };

            // 3 misiones distintas sin repetir tipo
            while (missions.Count < 3 && pool.Count > 0)
            {
                int idx = rng.Next(pool.Count);
                pool[idx](rng);
                pool.RemoveAt(idx);
            }

            moneyInitialized = false; // re-inicializar el balance base del dia
            OnMissionsChanged?.Invoke();
        }

        // ---------- seguimiento ----------

        void OnObjectUsed(PlaceableObject obj)
        {
            if (obj == null) return;
            AddProgress(MissionKind.UseObjects, 1f);
        }

        void OnXpGained(SkillType type, float amount)
        {
            AddProgress(MissionKind.SkillXp, amount);
        }

        void OnMoneyChanged(float newBalance)
        {
            if (!moneyInitialized) { lastMoney = newBalance; moneyInitialized = true; return; }
            float delta = newBalance - lastMoney;
            lastMoney = newBalance;
            if (delta > 0f) AddProgress(MissionKind.EarnMoney, delta);
        }

        void OnTravel(LocationZone from, LocationZone to)
        {
            AddProgress(MissionKind.Travel, 1f);
        }

        void AddProgress(MissionKind kind, float amount)
        {
            bool changed = false;
            foreach (var m in missions)
            {
                if (m.done || m.kind != kind) continue;
                m.progress = Mathf.Min(m.target, m.progress + amount);
                changed = true;
                if (m.progress >= m.target)
                {
                    m.done = true;
                    EconomySystem.Instance?.Add(m.reward);
                    totalCompleted++;
                    if (totalCompleted >= 5) AchievementSystem.Instance?.Unlock("misionero");
                }
            }
            if (changed) OnMissionsChanged?.Invoke();
        }
    }
}
