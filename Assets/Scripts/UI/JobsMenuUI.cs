using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Menú de empleos: un botón por oferta del JobCatalog. Los empleos con
    /// requisito de habilidad sin cumplir aparecen bloqueados con el motivo.
    /// Montaje igual que StoreUI: contenedor + botón plantilla.
    /// </summary>
    public class JobsMenuUI : MonoBehaviour
    {
        public RectTransform listContainer;
        public Button buttonTemplate;

        void Start() { BuildMenu(); }
        void OnEnable() { if (JobCatalog.Instance != null) BuildMenu(); }

        public void BuildMenu()
        {
            if (listContainer == null || buttonTemplate == null || JobCatalog.Instance == null) return;
            buttonTemplate.gameObject.SetActive(false);

            foreach (Transform child in listContainer) { if (child != buttonTemplate.transform) Destroy(child.gameObject); }

            foreach (var offer in JobCatalog.Instance.offers)
            {
                bool unlocked = JobCatalog.Instance.MeetsRequirements(offer);

                var btn = Instantiate(buttonTemplate, listContainer);
                btn.gameObject.SetActive(true);
                btn.interactable = unlocked;

                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = unlocked
                    ? $"{offer.job.name} - ${offer.job.hourlyPay:0}/hora"
                    : $"{offer.job.name} - requiere {offer.requiredSkill} nivel {offer.requiredLevel}";

                var captured = offer;
                btn.onClick.AddListener(() =>
                {
                    JobSystem.Instance?.SetJob(captured.job);
                    BuildMenu();
                });
            }
        }
    }
}
