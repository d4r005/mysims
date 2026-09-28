using System;
using UnityEngine;

namespace MySims
{
    /// <summary>
    /// Calendario del mundo: dia de la semana y dias festivos.
    /// El dia 1 del juego es Lunes. Los festivos no se trabaja.
    /// </summary>
    public class CalendarSystem : MonoBehaviour
    {
        public static CalendarSystem Instance { get; private set; }

        static readonly string[] DayNames = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" };

        public bool IsWeekend => DayIndex == 5 || DayIndex == 6;

        [Tooltip("Cada N dias es festivo. 0 = desactivado")]
        public int holidayEveryDays = 0;

        public int DayIndex { get; private set; }
        public string DayName => DayNames[DayIndex];
        public bool IsHoliday { get; private set; }

        public event Action<int, string> OnNewDay;

        void Awake()
        {
            Instance = this;
            DayIndex = 0;
        }

        void OnEnable() { TimeSystem.Instance.OnDayChanged += HandleDayChanged; }
        void OnDisable() { if (TimeSystem.Instance != null) TimeSystem.Instance.OnDayChanged -= HandleDayChanged; }

        void HandleDayChanged(int day)
        {
            DayIndex = (day - 1) % 7;
            IsHoliday = holidayEveryDays > 0 && day % holidayEveryDays == 0;
            OnNewDay?.Invoke(day, DayName);
        }
    }
}
