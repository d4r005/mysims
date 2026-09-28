using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum FamilyStage { Soltero, Cortejo, Comprometido, Casado }

    [Serializable]
    public class Child
    {
        public string name;
        public int ageDays;
        public GameObject gameObject; // instanciado al cargar, no se guarda
    }

    /// <summary>
    /// Pack Vida Familiar: etapas de relacion con el vecino mas cercano,
    /// compromiso, boda e hijos que crecen con los dias. Similar a la expansion
    /// de generaciones de Sims 3.
    /// </summary>
    public class FamilySystem : MonoBehaviour
    {
        public static FamilySystem Instance { get; private set; }

        [Header("Costos")]
        public float cortejoRelationRequired = 30f;
        public float compromisoCost = 300f;
        public float compromisoRelationRequired = 60f;
        public float bodaCost = 800f;
        public float bodaRelationRequired = 80f;
        public float costoHijo = 200f;

        [Header("Estado")]
        public FamilyStage stage = FamilyStage.Soltero;
        public string partnerName = "";
        public List<Child> children = new List<Child>();

        [Header("Prefabs")]
        [Tooltip("Prefab del hijo. Se busca tambien en PrefabRegistry por nombre Child.")]
        public GameObject childPrefab;

        [Header("Dinastia (pack Dinastias y Linajes)")]
        public int generation = 1;
        public float legacyScore;

        public event Action<FamilyStage> OnStageChanged;
        public event Action<Child> OnChildBorn;
        public event Action<int> OnGenerationAdvanced;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnDayChanged += GrowChildren; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= GrowChildren; }

        /// <summary>Avanza la relacion con el vecino mas cercano. Devuelve false si no se pudo.</summary>
        public bool TryAdvanceStage()
        {
            var npc = NearestPartner();
            var dialogue = DialogueSystem.Instance;
            if (npc == null || dialogue == null) return false;

            float rel = dialogue.GetRelation(npc);
            var economy = EconomySystem.Instance;

            switch (stage)
            {
                case FamilyStage.Soltero:
                    if (rel < cortejoRelationRequired) return false;
                    stage = FamilyStage.Cortejo;
                    partnerName = npc.npcName;
                    break;

                case FamilyStage.Cortejo:
                    if (rel < compromisoRelationRequired) return false;
                    if (economy == null || !economy.TrySpend(compromisoCost)) return false;
                    stage = FamilyStage.Comprometido;
                    break;

                case FamilyStage.Comprometido:
                    if (rel < bodaRelationRequired) return false;
                    if (economy == null || !economy.TrySpend(bodaCost)) return false;
                    stage = FamilyStage.Casado;
                    AchievementSystem.Instance?.Unlock("boda");
                    break;

                case FamilyStage.Casado:
                    return false;
            }

            OnStageChanged?.Invoke(stage);
            return true;
        }

        /// <summary>Tener un hijo. Solo casado y pagando el costo.</summary>
        public bool TryHaveChild(string childName)
        {
            if (stage != FamilyStage.Casado) return false;
            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(costoHijo)) return false;

            var child = new Child { name = string.IsNullOrEmpty(childName) ? "Bebé " + (children.Count + 1) : childName, ageDays = 0 };
            children.Add(child);
            SpawnChildVisual(child);
            OnChildBorn?.Invoke(child);

            if (children.Count >= 2) AchievementSystem.Instance?.Unlock("familia");
            return true;
        }

        void SpawnChildVisual(Child child)
        {
            GameObject prefab = childPrefab;
            if (prefab == null) prefab = PrefabRegistry.Instance?.GetPrefab("Child");
            if (prefab == null) return;

            var player = GameManager.Instance?.playerCharacter;
            Vector3 pos = player != null ? player.transform.position + Vector3.right * 1.5f : Vector3.zero;
            child.gameObject = Instantiate(prefab, pos, Quaternion.identity);
        }

        /// <summary>Restaura un hijo guardado sin costo. Usado al cargar partida.</summary>
        public void TryRestoreChild(string name, int ageDays)
        {
            var child = new Child { name = name, ageDays = ageDays };
            children.Add(child);
            SpawnChildVisual(child);
        }

        void GrowChildren(int day)
        {
            foreach (var c in children) c.ageDays++;
        }

        NPCRoutine NearestPartner()
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) return null;

            NPCRoutine nearest = null;
            float min = 4f;
            foreach (var npc in UnityEngine.Object.FindObjectsOfType<NPCRoutine>())
            {
                float d = Vector3.Distance(player.transform.position, npc.transform.position);
                if (d <= min) { min = d; nearest = npc; }
            }
            return nearest;
        }

        /// <summary>Pasa el legado a la siguiente generacion. Pack Dinastias y Linajes.</summary>
        public void AdvanceGeneration()
        {
            float achievementsBonus = AchievementSystem.Instance != null ? AchievementSystem.Instance.Unlocked.Count * 10f : 0f;
            float moneyBonus = EconomySystem.Instance != null ? EconomySystem.Instance.money * 0.01f : 0f;
            legacyScore += achievementsBonus + moneyBonus;
            generation++;
            OnGenerationAdvanced?.Invoke(generation);
            if (generation >= 3) AchievementSystem.Instance?.Unlock("dinastia");
        }

        /// <summary>Cita romantica con la pareja mas cercana. Pack Noctambulos / Viva el Amor.</summary>
        public bool GoOnDate(float cost = 50f)
        {
            var npc = NearestPartner();
            if (npc == null) return false;
            var economy = EconomySystem.Instance;
            if (economy == null || !economy.TrySpend(cost)) return false;

            DialogueSystem.Instance?.TryStartConversationWith(npc);
            NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.4f);
            AchievementSystem.Instance?.Unlock("cita_romantica");
            return true;
        }

        public string StageLabel =>
            stage == FamilyStage.Soltero ? "Soltero"
            : stage == FamilyStage.Cortejo ? "Saliendo con " + partnerName
            : stage == FamilyStage.Comprometido ? "Comprometido con " + partnerName
            : "Casado con " + partnerName;
    }
}
