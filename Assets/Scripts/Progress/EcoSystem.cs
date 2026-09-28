using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Pack Vida Ecologica: reciclar sube el puntaje ecologico del vecindario;
    /// alto puntaje abarata servicios, bajo los encarece.
    /// </summary>
    public class EcoSystem : MonoBehaviour
    {
        public static EcoSystem Instance { get; private set; }

        [Range(-100, 100)] public float ecoScore;
        public event Action<float> OnEcoScoreChanged;

        void Awake() { Instance = this; }

        public void Recycle(float amount = 5f)
        {
            ecoScore = Mathf.Clamp(ecoScore + amount, -100f, 100f);
            OnEcoScoreChanged?.Invoke(ecoScore);
            SkillSystem.Instance?.GainXP(SkillType.Logica, 1f);
            if (ecoScore >= 80f) AchievementSystem.Instance?.Unlock("eco");
        }

        public void Pollute(float amount = 5f)
        {
            ecoScore = Mathf.Clamp(ecoScore - amount, -100f, 100f);
            OnEcoScoreChanged?.Invoke(ecoScore);
        }

        public float ServiceCostMultiplier => Mathf.Lerp(1.3f, 0.85f, (ecoScore + 100f) / 200f);
    }
}
