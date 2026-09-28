using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Personalización del sim: color de piel, cabello, playera y pantalón.
    /// Con un personaje primitivo aplica los colores a los materiales asignados;
    /// con un modelo real se conectan los renderers correctos en el Inspector.
    /// El color se guarda en la partida como hex.
    /// </summary>
    public class CharacterCustomizer : MonoBehaviour
    {
        public static CharacterCustomizer Instance { get; private set; }

        [Header("Renderers")]
        public Renderer bodyRenderer;   // piel
        public Renderer hairRenderer;
        public Renderer shirtRenderer;
        public Renderer pantsRenderer;

        [Header("Colores actuales")]
        public Color skinColor = new Color(0.85f, 0.66f, 0.50f);
        public Color hairColor = new Color(0.20f, 0.12f, 0.08f);
        public Color shirtColor = new Color(0.20f, 0.50f, 0.80f);
        public Color pantsColor = new Color(0.25f, 0.25f, 0.30f);

        void Awake() { Instance = this; Apply(); }

        public void Apply()
        {
            SetColor(bodyRenderer, skinColor);
            SetColor(hairRenderer, hairColor);
            SetColor(shirtRenderer, shirtColor);
            SetColor(pantsRenderer, pantsColor);
        }

        void SetColor(Renderer r, Color c)
        {
            if (r == null) return;
            foreach (var m in r.materials) m.color = c;
        }

        public void Randomize()
        {
            skinColor = Random.ColorHSV(0.02f, 0.08f, 0.4f, 0.8f, 0.5f, 0.95f);
            hairColor = Random.ColorHSV(0f, 1f, 0.2f, 0.8f, 0.1f, 0.5f);
            shirtColor = Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.5f, 0.95f);
            pantsColor = Random.ColorHSV(0f, 1f, 0.2f, 0.6f, 0.2f, 0.6f);
            Apply();
        }
    }
}
