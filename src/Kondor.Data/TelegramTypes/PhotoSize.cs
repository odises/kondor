using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class PhotoSize
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }
}