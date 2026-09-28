using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum DialogueTopic
    {
        Saludo,
        Cumplido,
        Chiste,
        CharlaProfunda
    }

    /// <summary>
    /// Conversaciones simples: cuando el jugador se acerca a un NPC con rutina,
    /// inician un dialogo de varios turnos que recupera Social y mejora la relación.
    /// La relación abre más temas y da bonos de amistad para eventos futuros.
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        public static DialogueSystem Instance { get; private set; }

        [Header("Configuracion")]
        public float metersToStartConversation = 2.5f;
        public int maxTurns = 5;
        public float secondsPerTurn = 1.5f;

        [Header("Ganancias por tema. La CharlaProfunda requiere relación.")]
        public float socialPerTurn = 8f;
        public float relationGainSaludo = 2f;
        public float relationGainCumplido = 4f;
        public float relationGainChiste = 5f;
        public float relationGainProfunda = 8f;
        public float relationNeededForProfunda = 40f;

        /// <summary>Puntuación de relación del jugador con cada NPC, por nombre.</summary>
        public Dictionary<string, float> Relationships { get; } = new Dictionary<string, float>();

        public event System.Action<NPCRoutine, DialogueTopic> OnTurnPlayed;

        void Awake() { Instance = this; }

        void Update()
        {
            // Auto-inicio: jugador con Social bajo cerca de un NPC conversador
            var needs = NeedsSystem.Instance;
            if (needs != null)
            {
                var social = needs.GetNeed(NeedType.Social);
                if (social != null && social.value < 45f)
                    TryStartConversationWith(FindNearbyNpc());
            }
        }

        NPCRoutine FindNearbyNpc()
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) return null;

            float min = metersToStartConversation;
            NPCRoutine nearest = null;
            foreach (var npc in FindObjectsOfType<NPCRoutine>())
            {
                float d = Vector3.Distance(player.transform.position, npc.transform.position);
                if (d <= min) { min = d; nearest = npc; }
            }
            return nearest;
        }

        public void TryStartConversationWith(NPCRoutine npc)
        {
            if (npc == null || npc.IsBusy) return;
            StartCoroutine(Conversation(npc));
        }

        IEnumerator Conversation(NPCRoutine npc)
        {
            var player = GameManager.Instance?.playerCharacter;
            if (player == null) yield break;

            npc.IsBusy = true;
            SetRelation(npc, GetRelation(npc) + relationGainSaludo);

            for (int turn = 0; turn < maxTurns; turn++)
            {
                DialogueTopic topic = PickTopic(npc);
                PlayTopic(npc, topic);
                OnTurnPlayed?.Invoke(npc, topic);
                yield return new WaitForSeconds(secondsPerTurn);

                var social = NeedsSystem.Instance?.GetNeed(NeedType.Social);
                if (social == null || social.value >= 95f) break;
            }

            npc.IsBusy = false;
        }

        DialogueTopic PickTopic(NPCRoutine npc)
        {
            float rel = GetRelation(npc);
            if (rel >= relationNeededForProfunda && Random.value < 0.5f) return DialogueTopic.CharlaProfunda;
            return (DialogueTopic)Random.Range(0, 3); // Saludo, Cumplido o Chiste
        }

        void PlayTopic(NPCRoutine npc, DialogueTopic topic)
        {
            if (NeedsSystem.Instance != null)
                NeedsSystem.Instance.Recover(NeedType.Social, socialPerTurn / 40f); // ~equivalente a 12 min de juego

            float gain = topic == DialogueTopic.Saludo ? relationGainSaludo
                       : topic == DialogueTopic.Cumplido ? relationGainCumplido
                       : topic == DialogueTopic.Chiste ? relationGainChiste
                       : relationGainProfunda;

            SetRelation(npc, GetRelation(npc) + gain);
            SkillSystem.Instance?.GainXP(SkillType.Carisma, 2f);
        }

        public float GetRelation(NPCRoutine npc) =>
            Relationships.TryGetValue(npc.npcName, out float v) ? v : 0f;

        void SetRelation(NPCRoutine npc, float value) =>
            Relationships[npc.npcName] = Mathf.Min(value, 100f);
    }
}
