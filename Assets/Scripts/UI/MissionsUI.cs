using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Panel de misiones diarias: lista las 3 misiones del dia con su avance.
    /// Se actualiza solo, suscrito a DailyMissionSystem.OnMissionsChanged.
    /// Colocar en un Text de la UI.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class MissionsUI : MonoBehaviour
    {
        Text label;

        void Start()
        {
            label = GetComponent<Text>();
            if (DailyMissionSystem.Instance != null)
                DailyMissionSystem.Instance.OnMissionsChanged += Refresh;
            Refresh();
        }

        void OnDestroy()
        {
            if (DailyMissionSystem.Instance != null)
                DailyMissionSystem.Instance.OnMissionsChanged -= Refresh;
        }

        void Refresh()
        {
            var ms = DailyMissionSystem.Instance;
            if (ms == null || label == null) return;
            string txt = "<b>Misiones de hoy</b>";
            foreach (var m in ms.missions)
            {
                string check = m.done ? "[OK] " : "";
                txt += $"\n{check}{m.detail} (${m.reward})";
                if (!m.done)
                    txt += $"\n     {Mathf.FloorToInt(m.progress)}/{Mathf.FloorToInt(m.target)}";
            }
            label.text = txt;
        }
    }
}
