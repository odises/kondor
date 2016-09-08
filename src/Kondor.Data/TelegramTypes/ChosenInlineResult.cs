using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class ChosenInlineResult
    {
        [JsonProperty("result_id")]
        public string ResultId { get; set; }

        [JsonProperty("from")]
        public User From { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("inline_message_id")]
        public string InlineMessageId { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
    }
}