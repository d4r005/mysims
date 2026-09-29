using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Fuerza la calidad grafica al arrancar: sombras activas, suaves y con buen
    /// alcance, anti-aliasing 4x y MSAA en la camara. Los QualitySettings por
    /// defecto (sobre todo en Android) traen las sombras apagadas y eso hace que
    /// la escena se vea plana y fea.
    /// </summary>
    public class GraphicsQuality : MonoBehaviour
    {
        void Awake()
        {
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowDistance = 60f;
            QualitySettings.shadowCascades = 2;
            QualitySettings.antiAliasing = 4;
            QualitySettings.pixelLightCount = 1;
            QualitySettings.softVegetation = true;

            if (Camera.main != null)
                Camera.main.allowMSAA = true;

            Debug.Log("MySims: calidad grafica aplicada (sombras suaves, 4x MSAA, sombras a 60m)");
        }
    }
}
