using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class InlineKeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("callback_data")]
        public string CallbackData { get; set; }

        [JsonProperty("switch_inline_query")]
        public string SwitchInlineQuery { get; set; }

        [JsonProperty("switch_inline_query_current_chat")]
        public string SwitchInlineQueryCurrentChat { get; set; }

        [JsonProperty("callback_game")]
        public CallbackGame CallbackGame { get; set; }
    }
}