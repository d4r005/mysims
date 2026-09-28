using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Botones de velocidad del tiempo: pausa / x1 / x2 / x4.
    /// Montaje: botones con este script, cada uno con speedIndex (0=pausa, 1=x1, 2=x2, 3=x4).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class TimeControlsUI : MonoBehaviour
    {
        [Tooltip("0 = pausa, 1 = x1, 2 = x2, 3 = x4")]
        public int speedIndex = 1;
        static readonly float[] speeds = { 0f, 1f, 2f, 4f };

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ApplySpeed);
        }

        void ApplySpeed()
        {
            if (TimeSystem.Instance == null) return;
            float s = speeds[Mathf.Clamp(speedIndex, 0, speeds.Length - 1)];
            TimeSystem.Instance.SetSpeed(s < 1f ? 1f : s);
            TimeSystem.Instance.enabled = s > 0f; // 0 = pausa (desactiva el reloj)
        }
    }
}
