using System;

namespace Kondor.Data
{
    public class QueryData
    {
        public QueryData(string command, string action, string data)
        {
            Command = command;
            Action = action;
            Data = data;
            Ticks = DateTime.Now.Ticks;
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
                return new QueryData(splitted[0], splitted[1], splitted[2]);
            }
        }

        protected static QueryData New(string command, string action, string data)
        {
            var queryData = new QueryData(command, action, data);
            return queryData;
        }

        public static string NewQueryString(string command, string action, string data)
        {
            var queryData = new QueryData(command, action, data);
            return queryData.ToString();
        }

        public override string ToString()
        {
            return $"{Command}:{Action}:{Data}:{Ticks}";
        }
    }
}
