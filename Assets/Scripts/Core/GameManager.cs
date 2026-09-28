using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Punto de entrada del juego. Orquesta los sistemas principales.
    /// Colocar en un GameObject "GameManager" en la escena.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Sistemas (asignar en Inspector)")]
        public TimeSystem timeSystem;
        public NeedsSystem playerNeeds;
        public NPCController playerCharacter;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            SaveManager.LoadGame(); // Cargar partida si existe
        }

        void OnApplicationPause(bool pause)
        {
            // En Android: guardar al pausar (cambio de app, bloqueo, etc.)
            if (pause) SaveManager.SaveGame();
        }

        void OnApplicationQuit()
        {
            SaveManager.SaveGame();
        }
    }
}
