using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Catálogo de muebles colocables. Llenar en el Inspector con todos los prefabs
    /// comprables/colocables. Guardado y carga usan el nombre del prefab como clave.
    /// </summary>
    public class PrefabRegistry : MonoBehaviour
    {
        public static PrefabRegistry Instance { get; private set; }

        [System.Serializable]
        public class Entry { public GameObject prefab; }

        public List<Entry> entries = new List<Entry>();
        Dictionary<string, GameObject> lookup;

        void Awake()
        {
            Instance = this;
            lookup = new Dictionary<string, GameObject>();
            foreach (var e in entries)
                if (e.prefab != null) lookup[e.prefab.name] = e.prefab;
        }

        public GameObject GetPrefab(string name) =>
            lookup.TryGetValue(name, out var prefab) ? prefab : null;
    }
}
