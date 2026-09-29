using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// HUD de simoleones: muestra "§ 500" y se actualiza solo,
    /// suscrito a EconomySystem.OnMoneyChanged. Colocar en un Text de la UI.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class MoneyHUD : MonoBehaviour
    {
        void OnEnable()
        {
            if (EconomySystem.Instance != null)
            {
                EconomySystem.Instance.OnMoneyChanged += Set;
                Set(EconomySystem.Instance.money);
            }
        }

        void OnDisable()
        {
            if (EconomySystem.Instance != null)
                EconomySystem.Instance.OnMoneyChanged -= Set;
        }

        void Start()
        {
            if (EconomySystem.Instance != null) Set(EconomySystem.Instance.money);
        }

        void Set(float amount)
        {
            GetComponent<Text>().text = "§ " + Mathf.FloorToInt(amount);
        }
    }
}
