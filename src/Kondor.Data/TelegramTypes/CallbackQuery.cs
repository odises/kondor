using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class CallbackQuery
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public User From { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("inline_message_id")]
        public string InlineMessageId { get; set; }

        [JsonProperty("chat_instance")]
        public string ChatInstance { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("game_short_name")]
        public string GameShortName { get; set; }
    }
}