using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class InlineQuery
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public User From { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("offset")]
        public string Offset { get; set; }
    }
}