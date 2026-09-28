using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Reloj del juego: hora del dia, contador de dias y multiplicador de velocidad.
    /// Emite OnHourChanged; los sistemas (necesidades, trabajo, suenos) se enganchan ahi.
    /// </summary>
    public class TimeSystem : MonoBehaviour
    {
        public static TimeSystem Instance { get; private set; }

        [Header("Tiempo")]
        [Range(0f, 23.99f)] public float startHour = 8f;
        public float minutesPerRealSecond = 2f;   // 1 seg real = 2 min de juego
        [Range(1f, 8f)] public float speedMultiplier = 1f;

        public event Action<int> OnHourChanged;
        public event Action<int> OnDayChanged;

        public float CurrentHour { get; private set; }
        public int CurrentDay { get; private set; } = 1;
        public bool IsNight => CurrentHour < 6f || CurrentHour >= 20f;

        int lastHour;

        void Awake()
        {
            Instance = this;
            CurrentHour = startHour;
            lastHour = Mathf.FloorToInt(CurrentHour);
        }

        void Update()
        {
            // Un dia de juego = 12 min reales a velocidad x1 (24h * 60 / 2)
            CurrentHour += (Time.deltaTime * minutesPerRealSecond / 60f) * speedMultiplier;

            if (CurrentHour >= 24f)
            {
                CurrentHour -= 24f;
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }

            int hour = Mathf.FloorToInt(CurrentHour);
            if (hour != lastHour)
            {
                lastHour = hour;
                OnHourChanged?.Invoke(hour);
            }
        }

        public void SetSpeed(float multiplier) => speedMultiplier = Mathf.Clamp(multiplier, 1f, 8f);

        /// <summary>Avanza el reloj varias horas. Usado por los viajes y esperas.</summary>
        public void AdvanceHours(float hours)
        {
            if (hours <= 0f) return;
            CurrentHour += hours;
            while (CurrentHour >= 24f)
            {
                CurrentHour -= 24f;
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }
            lastHour = Mathf.FloorToInt(CurrentHour);
            OnHourChanged?.Invoke(lastHour);
        }

        public void SetFromLoad(int day, float hour)
        {
            CurrentDay = day;
            CurrentHour = hour;
            lastHour = Mathf.FloorToInt(hour);
        }
    }
}
