using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Game
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("photo")]
        public PhotoSize[] Photo { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("text_entities")]
        public MessageEntity[] TextEntities { get; set; }
        [JsonProperty("animation")]
        public Animation Animation { get; set; }
    }
}