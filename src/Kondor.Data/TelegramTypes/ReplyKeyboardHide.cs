using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class ReplyKeyboardHide
    {
        [JsonProperty("hide_keyboard")]
        public bool HideKeyboard => true;

        [JsonProperty("selective")]
        public bool Selective { get; set; }
    }
}