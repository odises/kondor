using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class KeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("request_contact")]
        public bool RequestContact { get; set; }

        [JsonProperty("request_location")]
        public bool RequestLocation { get; set; }
    }
}