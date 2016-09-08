using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class InlineKeyboardMarkup
    {
        [JsonProperty("inline_keyboard")]
        public InlineKeyboardButton[,] InlineKeyboard { get; set; }
    }
}