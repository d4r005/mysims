using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Tienda de muebles: genera un botón por cada prefab del PrefabRegistry,
    /// muestra nombre y precio, y al comprar descuenta de la billetera y
    /// entra en modo construcción para colocarlo. Si no alcanza el dinero, no compra.
    /// Montaje: un panel con ScrollRect, un botón plantilla y este script.
    /// </summary>
    public class StoreUI : MonoBehaviour
    {
        [Header("Referencias")]
        public RectTransform listContainer;
        public Button buttonTemplate;

        [Header("Margen de precio de tienda")]
        [Tooltip("Multiplicador sobre el precio base del mueble")]
        public float priceMultiplier = 1f;

        void Start() { BuildCatalog(); }

        public void BuildCatalog()
        {
            if (listContainer == null || buttonTemplate == null) return;
            buttonTemplate.gameObject.SetActive(false);

            foreach (Transform child in listContainer) { if (child != buttonTemplate.transform) Destroy(child.gameObject); }

            var registry = PrefabRegistry.Instance;
            if (registry == null) { Debug.LogWarning("StoreUI: falta PrefabRegistry en la escena"); return; }

            foreach (var entry in registry.entries)
            {
                var prefab = entry.prefab;
                if (prefab == null) continue;

                var placeable = prefab.GetComponent<PlaceableObject>();
                float basePrice = placeable != null ? placeable.price : 0f;
                float finalPrice = basePrice * priceMultiplier;

                var btn = Instantiate(buttonTemplate, listContainer);
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                    $"{prefab.name} - ${finalPrice:0}";

                var captured = prefab;
                btn.onClick.AddListener(() => TryBuy(captured, finalPrice));
            }
        }

        void TryBuy(GameObject prefab, float price)
        {
            var economy = EconomySystem.Instance;
            if (economy == null || GridPlacement.Instance == null) return;

            if (price > 0f && !economy.TrySpend(price))
            {
                Debug.Log("Dinero insuficiente para comprar " + prefab.name);
                return; // la UI puede engancharse a OnMoneyChanged para feedback
            }

            GridPlacement.Instance.EnterBuildMode(prefab);
        }
    }
}
