using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// UI estilo Sims: una barra (Slider) por necesidad, con ícono y color que cambia
    /// según el nivel (verde → amarillo → rojo). Se conecta sola a NeedsSystem.
    /// Montaje: crear un panel con 5 sliders, cada uno con nombre de objeto "Bar_Hambre",
    /// "Bar_Energia", "Bar_Social", "Bar_Diversión", "Bar_Higiene".
    /// </summary>
    public class NeedsUIController : MonoBehaviour
    {
        [System.Serializable]
        public class NeedBar
        {
            public NeedType type;
            public Slider slider;
            public Image fillImage;
        }

        public List<NeedBar> bars = new List<NeedBar>();

        [Header("Colores por nivel")]
        public Color goodColor = new Color(0.30f, 0.80f, 0.35f);
        public Color mediumColor = new Color(0.95f, 0.80f, 0.20f);
        public Color criticalColor = new Color(0.90f, 0.25f, 0.20f);

        void OnEnable()
        {
            if (NeedsSystem.Instance != null)
                NeedsSystem.Instance.OnNeedChanged += UpdateBar;
        }

        void OnDisable()
        {
            if (NeedsSystem.Instance != null)
                NeedsSystem.Instance.OnNeedChanged -= UpdateBar;
        }

        void Start()
        {
            // Primera pintura de todas las barras
            if (NeedsSystem.Instance != null)
                foreach (var need in NeedsSystem.Instance.needs)
                    UpdateBar(need);
        }

        void UpdateBar(Need need)
        {
            var bar = bars.Find(b => b.type == need.type);
            if (bar?.slider == null) return;

            bar.slider.value = need.value / 100f;

            if (bar.fillImage != null)
            {
                if (need.IsCritical) bar.fillImage.color = criticalColor;
                else if (need.value < 50f) bar.fillImage.color = mediumColor;
                else bar.fillImage.color = goodColor;
            }
        }
    }
}
