using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Billetera del jugador. Todo ingreso/gasto pasa por aquí para que la UI
    /// y el guardado se enteren. Emite OnMoneyChanged.
    /// </summary>
    public class EconomySystem : MonoBehaviour
    {
        public static EconomySystem Instance { get; private set; }

        [Header("Estado")]
        public float money = 500f;

        public event Action<float> OnMoneyChanged;

        void Awake() { Instance = this; }

        public void Add(float amount)
        {
            if (amount <= 0f) return;
            money += amount;
            OnMoneyChanged?.Invoke(money);
        }

        public bool TrySpend(float amount)
        {
            if (amount > money) return false;
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            return true;
        }
    }
}
