using System.IO;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Guardado local en JSON (Application.persistentDataPath funciona en Android).
    /// La nube (backend) viene en la fase 3 del roadmap.
    /// </summary>
    public static class SaveManager
    {
        static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

        public static void SaveGame()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.playerCharacter == null) return;

            var data = new SaveData
            {
                day = TimeSystem.Instance.CurrentDay,
                hour = TimeSystem.Instance.CurrentHour,
                playerPosition = ToSerializable(gm.playerCharacter.transform.position)
            };

            foreach (var need in gm.playerNeeds.needs)
                data.needs.Add(new NeedData { type = (int)need.type, value = need.value });

            foreach (var obj in PlaceableObject.All)
                data.placedObjects.Add(new PlacedObjectData
                {
                    prefabName = obj.name.Replace("(Clone)", ""),
                    position = ToSerializable(obj.transform.position),
                    rotationY = obj.transform.eulerAngles.y
                });

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"Partida guardada en {SavePath}");
        }

        public static bool LoadGame()
        {
            if (!File.Exists(SavePath)) return false;

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            var gm = GameManager.Instance;
            if (gm == null) return false;

            TimeSystem.Instance.StartCoroutine(RestoreNextFrame(data));
            return true;
        }

        static System.Collections.IEnumerator RestoreNextFrame(SaveData data)
        {
            yield return null; // esperar un frame a que todo este inicializado

            var gm = GameManager.Instance;
            TimeSystem.Instance.SetFromLoad(data.day, data.hour);
            gm.playerCharacter.transform.position = FromSerializable(data.playerPosition);

            foreach (var nd in data.needs)
            {
                var need = gm.playerNeeds.GetNeed((NeedType)nd.type);
                if (need != null) need.value = nd.value;
            }
            // Nota: los muebles colocados se restauran en la fase 2 (hace falta un registro de prefabs)
        }

        static Vector3Serializable ToSerializable(Vector3 v) =>
            new Vector3Serializable { x = v.x, y = v.y, z = v.z };

        static Vector3 FromSerializable(Vector3Serializable v) =>
            v == null ? Vector3.zero : new Vector3(v.x, v.y, v.z);
    }
}
