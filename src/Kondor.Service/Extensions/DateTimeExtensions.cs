using System;

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
    }
}
