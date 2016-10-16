using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class TelegramUpdate : JsonSerializable
    {
        [JsonProperty("update_id")]
        public int UpdateId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("edited_message")]
        public Message EditedMessage { get; set; }

        [JsonProperty("inline_query")]
        public InlineQuery InlineQuery { get; set; }

        [JsonProperty("chosen_inline_result")]
        public ChosenInlineResult ChosenInlineResult { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }
}
