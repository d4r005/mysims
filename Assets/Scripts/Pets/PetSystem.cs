using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Pack Mascotas: adopcion y cuidado. Los perros y gatos son prefabs con el
    /// componente Pet. Acariciar a la mascota mas cercana recupera Diversión.
    /// </summary>
    public class PetSystem : MonoBehaviour
    {
        public static PetSystem Instance { get; private set; }

        [Header("Costos de adopcion")]
        public float costoPerro = 250f;
        public float costoGato = 180f;

        [Tooltip("Prefabs de mascota. Si quedan vacios se buscan en PrefabRegistry por nombre Perro y Gato.")]
        public GameObject perroPrefab;
        public GameObject gatoPrefab;

        void Awake() { Instance = this; }

        public bool TryAdopt(Pet.Species species, string petName)
        {
            var economy = EconomySystem.Instance;
            float cost = species == Pet.Species.Perro ? costoPerro : costoGato;
            if (economy == null || !economy.TrySpend(cost)) return false;

            GameObject prefab = species == Pet.Species.Perro ? perroPrefab : gatoPrefab;
            if (prefab == null) prefab = PrefabRegistry.Instance?.GetPrefab(species == Pet.Species.Perro ? "Perro" : "Gato");
            if (prefab == null) { Debug.LogWarning("PetSystem: falta prefab de mascota"); return false; }

            var player = GameManager.Instance?.playerCharacter;
            Vector3 pos = player != null ? player.transform.position + Vector3.right * 1.5f : Vector3.zero;
            var petGo = Instantiate(prefab, pos, Quaternion.identity);

            var pet = petGo.GetComponent<Pet>();
            if (pet == null) pet = petGo.AddComponent<Pet>();
            pet.species = species;
            if (!string.IsNullOrEmpty(petName)) pet.petName = petName;

            return true;
        }

        /// <summary>Restaura una mascota guardada sin costo.</summary>
        public void RestorePet(PetData pd)
        {
            var species = (Pet.Species)pd.species;
            GameObject prefab = species == Pet.Species.Perro ? perroPrefab : gatoPrefab;
            if (prefab == null) prefab = PrefabRegistry.Instance?.GetPrefab(species == Pet.Species.Perro ? "Perro" : "Gato");
            if (prefab == null) return;

            var petGo = Instantiate(prefab);
            var pet = petGo.GetComponent<Pet>();
            if (pet == null) pet = petGo.AddComponent<Pet>();
            pet.species = species;
            pet.petName = pd.petName;
            pet.carino = pd.carino;
            pet.hambre = pd.hambre;
        }

        /// <summary>Acaricia la mascota mas cercana al sim. Llamado por el input tactil.</summary>
        public bool PetNearest()
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) return false;

            foreach (var pet in Pet.All)
            {
                if (Vector3.Distance(player.transform.position, pet.transform.position) <= 2.5f)
                {
                    pet.PetAnimal();
                    return true;
                }
            }
            return false;
        }
    }
}
