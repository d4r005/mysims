using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum LocationType { Ciudad, Playa, Pueblo, Campo }

    [Serializable]
    public class LocationInfo
    {
        public string id = "monterrey";
        public string displayName = "Monterrey";
        public string country = "Mexico";
        public LocationType type = LocationType.Ciudad;
        [Tooltip("Costo del viaje hasta esta ubicacion")]
        public float travelCost = 100f;
        [Tooltip("Horas de juego que toma el viaje")]
        public float travelHours = 4f;
        public string description = "";
    }

    /// <summary>
    /// Una zona del mundo: cada ubicacion tiene su zona con contenido propio
    /// y punto de aparición. Solo la zona activa se ve; viajar desactiva la
    /// actual y activa la destino. El contenido de cada zona va en zoneRoot.
    /// </summary>
    public class LocationZone : MonoBehaviour
    {
        public static readonly List<LocationZone> All = new List<LocationZone>();

        public LocationInfo info = new LocationInfo();
        public Transform spawnPoint;
        [Tooltip("Objeto padre con todo el contenido de la zona. Se activa o desactiva al viajar.")]
        public GameObject zoneRoot;

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);

        public void SetActive(bool active)
        {
            if (zoneRoot != null) zoneRoot.SetActive(active);
        }

        public Vector3 SpawnPosition =>
            spawnPoint != null ? spawnPoint.position : transform.position;
    }
}
