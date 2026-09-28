using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum HousingType { CasaPropia, Departamento }

    [Serializable]
    public class Roommate { public string name; public float shareOfRent = 0.5f; }

    /// <summary>
    /// Pack Comparten Piso / Se Alquila / Urbanitas: el sim puede vivir en un
    /// departamento de renta compartida con roommates que dividen el costo, o
    /// ser propietario. La renta se cobra cada mes de juego (30 dias).
    /// </summary>
    public class HousingSystem : MonoBehaviour
    {
        public static HousingSystem Instance { get; private set; }

        public HousingType housingType = HousingType.CasaPropia;
        public float monthlyRent = 200f;
        public List<Roommate> roommates = new List<Roommate>();

        public event Action OnEvicted;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnDayChanged += HandleDayChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= HandleDayChanged; }

        void HandleDayChanged(int day)
        {
            if (housingType == HousingType.CasaPropia || day % 30 != 0) return;

            float share = 1f;
            foreach (var r in roommates) share -= r.shareOfRent;
            float toPay = monthlyRent * Mathf.Max(0f, share);

            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(toPay)) OnEvicted?.Invoke();
        }

        public void AddRoommate(string name, float shareOfRent) =>
            roommates.Add(new Roommate { name = name, shareOfRent = shareOfRent });

        public void MoveToApartment(float rent)
        {
            housingType = HousingType.Departamento;
            monthlyRent = rent;
            roommates.Clear();
        }
    }
}
