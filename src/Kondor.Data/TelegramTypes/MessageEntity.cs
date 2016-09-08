using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class MessageEntity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
