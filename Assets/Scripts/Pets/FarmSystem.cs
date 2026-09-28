using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    /// <summary>Pack Rancho de Caballos: caballo que se puede montar y cuidar.</summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Horse : MonoBehaviour
    {
        public static readonly List<Horse> All = new List<Horse>();
        public string horseName = "Relampago";
        [Range(0, 100)] public float bondLevel = 30f;

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);

        public void Ride()
        {
            bondLevel = Mathf.Min(100f, bondLevel + 5f);
            SkillSystem.Instance?.GainXP(SkillType.Fitness, 4f);
            NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.3f);
            if (bondLevel >= 80f) AchievementSystem.Instance?.Unlock("jinete");
        }
    }

    [Serializable]
    public class Crop
    {
        public string name = "Maiz";
        public float growthDays = 4f;
        public float daysPlanted;
        public float sellPrice = 8f;
        public bool ReadyToHarvest => daysPlanted >= growthDays;
    }

    /// <summary>Pack Vida en el Pueblo: cultivos que crecen con los dias y se venden.</summary>
    public class FarmSystem : MonoBehaviour
    {
        public static FarmSystem Instance { get; private set; }

        public List<Crop> plantedCrops = new List<Crop>();
        public float horseAdoptionCost = 400f;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnDayChanged += GrowCrops; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= GrowCrops; }

        void GrowCrops(int day) { foreach (var c in plantedCrops) c.daysPlanted++; }

        public void Plant(string cropName, float growthDays, float sellPrice) =>
            plantedCrops.Add(new Crop { name = cropName, growthDays = growthDays, sellPrice = sellPrice });

        public float HarvestAll()
        {
            float total = 0f;
            plantedCrops.RemoveAll(c => { if (!c.ReadyToHarvest) return false; total += c.sellPrice; return true; });
            if (total > 0f) EconomySystem.Instance?.Add(total);
            return total;
        }

        public bool TryAdoptHorse(GameObject horsePrefab, Vector3 position)
        {
            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(horseAdoptionCost) || horsePrefab == null) return false;
            Object.Instantiate(horsePrefab, position, Quaternion.identity);
            return true;
        }
    }
}
