using System;
using System.Globalization;

namespace Kondor.Service.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetRounded(this DateTime value)
        {
            var datetime = value;

            var minutes = datetime.Minute;
            var seconds = datetime.Second;
            var miliseconds = datetime.Millisecond;


            DateTime rounded;
            if (datetime.Minute > 30)
            {
                var mins = 60 - minutes;
                
                rounded = datetime.AddMinutes(mins).AddSeconds(-seconds).AddMilliseconds(-miliseconds);
            }
            else
            {
                rounded = datetime.AddMinutes(-minutes).AddSeconds(-seconds).AddMilliseconds(-miliseconds);
            }

            return rounded;
        }

        public static Tuple<int, string> Humanize(this DateTime value)
        {
            if (value <= DateTime.Now)
            {
                return new Tuple<int, string>(0, value.ToString(CultureInfo.InvariantCulture));
            }
            var diff = value - DateTime.Now;

            if (diff.TotalSeconds < 60)
            {
                return new Tuple<int, string>((int)diff.TotalSeconds, StringResources.HumanizerSecondsFromNow);
            }
            if (diff.TotalMinutes < 60)
            {
                return new Tuple<int, string>((int)diff.TotalMinutes, StringResources.HumanizerMinutesFromNow);
            }
            if (diff.TotalHours < 72)
            {
                return new Tuple<int, string>((int)diff.TotalHours, StringResources.HumanizerHoursFromNow);
            }

            return new Tuple<int, string>((int)diff.TotalDays, StringResources.HumanizerDaysFromNow);
        }
    }
}
