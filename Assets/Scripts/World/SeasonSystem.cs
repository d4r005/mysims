using System;
using UnityEngine;

namespace MySims
{
    public enum Season { Primavera, Verano, Otono, Invierno }
    public enum Weather { Soleado, Nublado, Lluvia, Nieve, Tormenta }

    /// <summary>
    /// Pack Estaciones + Clima (Y las Cuatro Estaciones, Escapada en la Nieve):
    /// el ano se divide en 4 estaciones que rotan cada N dias. El clima varia
    /// segun la estacion (nieve solo en invierno, tormentas en verano).
    /// </summary>
    public class SeasonSystem : MonoBehaviour
    {
        public static SeasonSystem Instance { get; private set; }

        [Header("Configuracion")]
        public int daysPerSeason = 7;
        public Season currentSeason = Season.Primavera;
        public Weather currentWeather = Weather.Soleado;

        public event Action<Season> OnSeasonChanged;
        public event Action<Weather> OnWeatherChanged;

        void Awake() { Instance = this; }
        void OnEnable() { TimeSystem.Instance.OnDayChanged += HandleDayChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= HandleDayChanged; }

        void HandleDayChanged(int day)
        {
            var season = (Season)(((day - 1) / daysPerSeason) % 4);
            if (season != currentSeason)
            {
                currentSeason = season;
                OnSeasonChanged?.Invoke(currentSeason);
                CalendarSystem.Instance?.MarkSeasonalHoliday(currentSeason);
            }
            RollWeather();
        }

        void RollWeather()
        {
            Weather w;
            switch (currentSeason)
            {
                case Season.Invierno: w = UnityEngine.Random.value < 0.4f ? Weather.Nieve : (UnityEngine.Random.value < 0.5f ? Weather.Nublado : Weather.Soleado); break;
                case Season.Primavera: w = UnityEngine.Random.value < 0.35f ? Weather.Lluvia : Weather.Soleado; break;
                case Season.Verano: w = UnityEngine.Random.value < 0.1f ? Weather.Tormenta : Weather.Soleado; break;
                default: w = UnityEngine.Random.value < 0.25f ? Weather.Nublado : Weather.Soleado; break;
            }
            if (w != currentWeather) { currentWeather = w; OnWeatherChanged?.Invoke(w); }
        }

        public bool IsSnowy(LocationZone zone) =>
            currentSeason == Season.Invierno && zone != null && zone.info.type != LocationType.Playa;
    }
}
