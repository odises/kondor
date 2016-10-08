using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Animation
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }
        [JsonProperty("thumb")]
        public PhotoSize Thumb { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }
}