using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Objeto colocable que satisface una necesidad: cama (Energia), refri (Hambre),
    /// ducha (Higiene), TV (Diversión), sofa social (Social).
    /// Requiere un Collider y un "interactionPoint" (Transform hijo) donde se para el personaje.
    /// </summary>
    public class PlaceableObject : MonoBehaviour
    {
        public static readonly List<PlaceableObject> All = new List<PlaceableObject>();

        /// <summary>Se dispara cada vez que un personaje empieza a usar este objeto.</summary>
        public static event System.Action<PlaceableObject> OnUsed;

        [Header("Configuracion")]
        public NeedType satisfies;
        public string displayName = "Mueble";
        public Transform interactionPoint;

        /// <summary>true si lo colocó el jugador en modo construcción (se guarda en la partida).</summary>
        public bool placedByPlayer;

        [Header("Habilidad")]
        [Tooltip("Habilidad que entrena al usarlo. Carisma se entrena conversando.")]
        public SkillType trainsSkill;
        [Tooltip("XP por segundo de uso")]
        public float skillXpPerUse = 1f;

        [Header("Mascotas")]
        [Tooltip("Si es true solo las mascotas pueden usarlo, ej. comedero.")]
        public bool isForPets;

        [Header("Tienda")]
        [Tooltip("Precio de compra en modo construcción. 0 = gratis")]
        public float price;

        NPCController currentUser;

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);

        public void StartUse(NPCController user)
        {
            currentUser = user;
            AudioManager.PlayUse(satisfies);
            OnUsed?.Invoke(this);
        }
        public void ContinueUse(NPCController user)
        {
            // Recuperar a ritmo de juego: recoverPerHour * (minutos de juego este frame / 60)
            var t = TimeSystem.Instance;
            float gameHours = (Time.deltaTime * t.minutesPerRealSecond * t.speedMultiplier) / 60f;
            NeedsSystem.Instance?.Recover(satisfies, gameHours);
        }
        public void StopUse(NPCController user) { if (currentUser == user) currentUser = null; }

        public static PlaceableObject FindClosestForNeed(NeedType type, Vector3 from)
        {
            return All.Where(o => o.satisfies == type && o.isForPets == false)
                      .OrderBy(o => Vector3.Distance(o.transform.position, from))
                      .FirstOrDefault();
        }
    }
}
