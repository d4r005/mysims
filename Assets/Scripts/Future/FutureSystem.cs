using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Pack Hacia el Futuro: la maquina del tiempo permite saltar dias al futuro
    /// cobrando intereses del ahorro, y se pueden comprar robots companeros.
    /// Requiere una zona futurista. La expansion numero 40.
    /// </summary>
    public class FutureSystem : MonoBehaviour
    {
        public static FutureSystem Instance { get; private set; }

        [Header("Costos")]
        public float timeTravelCost = 1000f;
        public float interestPerDay = 0.05f;   // 5 por ciento diario del ahorro
        public float robotCost = 1500f;

        [Header("Estado")]
        public bool futureUnlocked;
        public int timeJumps;
        public GameObject robotPrefab;   // si queda null se busca en PrefabRegistry por Robot

        public event Action<int> OnTimeJump;

        void Awake() { Instance = this; }

        /// <summary>Desbloquear el futuro visitando la ciudad futurista. Llamar al llegar a NeoCiudad.</summary>
        public void UnlockFuture()
        {
            if (futureUnlocked) return;
            futureUnlocked = true;
            AchievementSystem.Instance?.Unlock("hacia_el_futuro");
        }

        /// <summary>Viajar al futuro N dias. Cobra costo y abona intereses por dia saltado.</summary>
        public bool TravelToFuture(int days)
        {
            if (!futureUnlocked || days <= 0) return false;

            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(timeTravelCost)) return false;

            // Interes compuesto por cada dia saltado
            economy.Add(economy.money * interestPerDay * days);

            TimeSystem.Instance.AdvanceHours(days * 24f);
            timeJumps++;
            OnTimeJump?.Invoke(timeJumps);
            AchievementSystem.Instance?.Unlock("viajero_tiempo");
            return true;
        }

        /// <summary>Comprar un robot companero.</summary>
        public bool BuyRobot(string robotName)
        {
            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(robotCost)) return false;

            GameObject prefab = robotPrefab;
            if (prefab == null) prefab = PrefabRegistry.Instance?.GetPrefab("Robot");
            if (prefab == null) { Debug.LogWarning("FutureSystem: falta prefab Robot"); economy.Add(robotCost); return false; }

            var player = GameManager.Instance?.playerCharacter;
            Vector3 pos = player != null ? player.transform.position + Vector3.right * 1.5f : Vector3.zero;
            var robotGo = Instantiate(prefab, pos, Quaternion.identity);

            var robot = robotGo.GetComponent<RobotCompanion>();
            if (robot == null) robot = robotGo.AddComponent<RobotCompanion>();
            if (!string.IsNullOrEmpty(robotName)) robot.robotName = robotName;

            AchievementSystem.Instance?.Unlock("dueno_robot");
            return true;
        }
    }
}
