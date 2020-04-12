using System;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public static class DateTimeExtension
    {
        public static DateTime IncreaseByDays(this int days, DateTime? pivot = null)
        {
            var results = null == pivot ? DateTime.Now : pivot.Value;
            return results.AddDays(days);
        }

        public static DateTime IncreaseByHours(this int hours, DateTime? pivot = null)
        {
            var results = null == pivot ? DateTime.Now : pivot.Value;
            return results.AddHours(hours);
        }

        public static DayOfWeek IncreaseByWeekOfDay(this int days, DayOfWeek? pivot = null)
        {
            const int TotalDaysOfWeek = 7;
            var remains = 0;
            var countOfWeeks = (int)((Math.Abs(days) + (int)DateTime.Today.DayOfWeek) / TotalDaysOfWeek);
            countOfWeeks = countOfWeeks > 0 ? countOfWeeks : 1;
            remains = (days + (int)(null == pivot ? DateTime.Today.DayOfWeek : pivot.Value) + countOfWeeks * TotalDaysOfWeek) % TotalDaysOfWeek;
            remains = remains < 0 ? -1 * remains : remains;
            return (DayOfWeek)remains;
        }

        public static DateTime RoundToMinutes(this DateTime datetime)
        {
            return DateTime.Parse(string.Format(@"{0} 00:00", datetime.ToString(@"MM/dd/yyyy HH:")));
        }
    }
}
