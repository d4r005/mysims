using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Mapa mundial: un botón por ubicación de la escena, agrupado por país.
    /// Muestra costo de viaje y permite viajar o establecer casa.
    /// Montaje: panel con contenedor y botón plantilla. Con Canvas Scaler funciona igual en móvil.
    /// </summary>
    public class WorldMapUI : MonoBehaviour
    {
        public RectTransform listContainer;
        public Button buttonTemplate;
        public Button liveHereButtonTemplate;

        TravelSystem travel;

        void Start()
        {
            travel = TravelSystem.Instance;
            if (travel == null) { Debug.LogWarning("WorldMapUI: falta TravelSystem"); return; }
            travel.OnTravel += (a, b) => BuildMap();
            travel.OnHomeChanged += id => BuildMap();
            BuildMap();
        }

        public void BuildMap()
        {
            if (listContainer == null || buttonTemplate == null) return;
            buttonTemplate.gameObject.SetActive(false);
            if (liveHereButtonTemplate != null) liveHereButtonTemplate.gameObject.SetActive(false);

            foreach (Transform child in listContainer)
                if (child != buttonTemplate.transform && (liveHereButtonTemplate == null || child != liveHereButtonTemplate.transform))
                    Destroy(child.gameObject);

            string lastCountry = null;
            foreach (var zone in LocationZone.All)
            {
                // separador por país
                if (zone.info.country != lastCountry)
                {
                    lastCountry = zone.info.country;
                    var sep = Instantiate(buttonTemplate, listContainer);
                    sep.interactable = false;
                    sep.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"<b>{zone.info.country}</b>";
                    sep.gameObject.SetActive(true);
                }

                bool isCurrent = zone.info.id == travel.currentLocationId;
                bool isHome = zone.info.id == travel.homeLocationId;
                float cost = travel.TravelCost(zone.info.id);

                var btn = Instantiate(buttonTemplate, listContainer);
                btn.gameObject.SetActive(true);
                btn.interactable = !isCurrent;
                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                    $"{zone.info.displayName}{(isHome ? " [casa]" : "")} - {cost:0}" +
                    (isCurrent ? " (estas aqui)" : "") + $"\n{zone.info.description}";

                var captured = zone;
                btn.onClick.AddListener(() => { travel.TravelTo(captured.info.id); });

                if (liveHereButtonTemplate != null && !isHome)
                {
                    var homeBtn = Instantiate(liveHereButtonTemplate, listContainer);
                    homeBtn.gameObject.SetActive(true);
                    var cap = zone;
                    homeBtn.onClick.AddListener(() => { travel.SetHome(cap.info.id); });
                }
            }
        }
    }
}
