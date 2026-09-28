using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Sistema de viajes del mundo. Cada ubicacion es una LocationZone; viajar cuesta
    /// dinero y horas de juego, desactiva la zona actual y activa la destino.
    /// El sim puede elegir casa: si viaja a su casa el pasaje cuesta la mitad.
    /// </summary>
    public class TravelSystem : MonoBehaviour
    {
        public static TravelSystem Instance { get; private set; }

        [Header("Estado")]
        public string currentLocationId = "monterrey";
        public string homeLocationId = "monterrey";
        public int locationsVisitedCount = 1;

        public LocationZone CurrentZone => FindZone(currentLocationId);
        public event Action<LocationZone, LocationZone> OnTravel;   // desde, hacia
        public event Action<string> OnHomeChanged;

        void Awake()
        {
            Instance = this;
            ActivateOnly(currentLocationId);
        }

        public static LocationZone FindZone(string id) =>
            LocationZone.All.Find(z => z.info.id == id);

        /// <summary>Cuesta del viaje desde la ubicación actual hasta id.</summary>
        public float TravelCost(string id)
        {
            if (id == currentLocationId) return 0f;
            var info = LocationCatalog.Get(id);
            if (info == null) return 0f;
            return id == homeLocationId ? info.travelCost * 0.5f : info.travelCost;
        }

        /// <summary>Viajar con costo y horas. Devuelve false si no hay zona o dinero.</summary>
        public bool TravelTo(string id)
        {
            var zone = FindZone(id);
            if (zone == null || id == currentLocationId) return false;

            float cost = TravelCost(id);
            var economy = EconomySystem.Instance;

            if (cost > 0f && (economy == null || !economy.TrySpend(cost)))
                return false;

            var from = CurrentZone;
            var info = LocationCatalog.Get(id);
            if (info != null && TimeSystem.Instance != null && info.travelHours > 0f)
                TimeSystem.Instance.AdvanceHours(info.travelHours); // el viaje toma tiempo

            ActivateOnly(id);
            currentLocationId = id;
            locationsVisitedCount = Mathf.Max(locationsVisitedCount, CountVisited());

            MovePlayerToSpawn(zone);
            OnTravel?.Invoke(from, zone);
            AchievementSystem.Instance?.NotifyTraveled();
            return true;
        }

        /// <summary>Teletransporte sin costo, usado al cargar la partida.</summary>
        public void TeleportTo(string id)
        {
            var zone = FindZone(id);
            if (zone == null) return;
            ActivateOnly(id);
            currentLocationId = id;
            MovePlayerToSpawn(zone);
        }

        /// <summary>Elegir en qué lugar del mundo vive el sim.</summary>
        public void SetHome(string id)
        {
            if (FindZone(id) == null) return;
            homeLocationId = id;
            OnHomeChanged?.Invoke(id);
        }

        int CountVisited()
        {
            // Aproximación: mientras no haya historial persistente, cuenta destinos alcanzables
            return Mathf.Max(1, locationsVisitedCount);
        }

        void MovePlayerToSpawn(LocationZone zone)
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player != null)
            {
                player.transform.position = zone.SpawnPosition;
                player.MoveTo(zone.SpawnPosition);
            }
        }

        void ActivateOnly(string id)
        {
            foreach (var z in LocationZone.All)
                z.SetActive(z.info.id == id);
        }
    }
}
