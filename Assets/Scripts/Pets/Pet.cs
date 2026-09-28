using UnityEngine;
using UnityEngine.AI;

namespace MySims
{
    /// <summary>
    /// Mascota del sim: perro o gato. Tiene hambre y carino propios, sigue al
    /// dueno cuando el carino es alto y deambula cuando no.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Pet : MonoBehaviour
    {
        public static readonly System.Collections.Generic.List<Pet> All = new System.Collections.Generic.List<Pet>();

        public enum Species { Perro, Gato }

        public Species species = Species.Perro;
        public string petName = "Firulais";
        [Range(0, 100)] public float carino = 50f;
        [Range(0, 100)] public float hambre = 60f;
        public float followDistance = 1.5f;
        public float wanderRadius = 4f;

        NavMeshAgent agent;
        Vector3 home;
        float wanderTimer;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            home = transform.position;
        }

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);

        void Update()
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) return;

            if (hambre < 35f) TryEat();
            else if (carino >= 40f) Follow(player.transform);
            else Wander();
        }

        void TryEat()
        {
            // Busca un comedero de mascotas cerca
            foreach (var obj in PlaceableObject.All)
            {
                if (!obj.isForPets) continue;
                if (Vector3.Distance(transform.position, obj.transform.position) < 3f)
                {
                    agent.SetDestination(obj.transform.position);
                    if (agent.remainingDistance < 0.8f) hambre = 100f;
                    return;
                }
            }
        }

        void Follow(Transform target)
        {
            float d = Vector3.Distance(transform.position, target.position);
            if (d > followDistance) agent.SetDestination(target.position);
        }

        void Wander()
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer < 4f) return;
            wanderTimer = 0f;
            Vector3 target = home + new Vector3(Random.Range(-wanderRadius, wanderRadius), 0f, Random.Range(-wanderRadius, wanderRadius));
            agent.SetDestination(target);
        }

        /// <summary>Acariciar: sube el carino de la mascota y la diversion del sim.</summary>
        public void PetAnimal()
        {
            carino = Mathf.Min(100f, carino + 10f);
            NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.4f);
            SkillSystem.Instance?.GainXP(SkillType.Carisma, 1f);
            if (carino >= 80f) AchievementSystem.Instance?.Unlock("mascota");
        }
    }
}
