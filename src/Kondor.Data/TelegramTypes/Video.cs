using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Video
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("duration")]
        public int Duration  { get; set; }

        [JsonProperty("thumb")]
        public PhotoSize Thumb { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }
}
