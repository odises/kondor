using System;

namespace Kondor.Service
{
    public class QueryData
    {
        public QueryData(string command, string action, string data, long ticks)
        {
            Command = command;
            Action = action;
            Data = data;
            Ticks = ticks;
        }

        public string Command { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public long Ticks { get; set; }

        public static QueryData Parse(string input)
        {
            var splitted = input.Split(new[] {':'}, StringSplitOptions.None);
            if (splitted.Length != 4)
            {
                throw new InvalidCastException();
            }
            else
            {
                return new QueryData(splitted[0], splitted[1], splitted[2], string.IsNullOrEmpty(splitted[3]) ? 0 : long.Parse(splitted[3]));
            }
        }

        protected static QueryData New(string command, string action, string data, long ticks)
        {
            var queryData = new QueryData(command, action, data, ticks);
            return queryData;
        }

        public static string NewQueryString(string command, string action, string data, long ticks)
        {
            var queryData = new QueryData(command, action, data, ticks);
            return queryData.ToString();
        }

        public override string ToString()
        {
            return $"{Command}:{Action}:{Data}:{Ticks}";
        }
    }
}
