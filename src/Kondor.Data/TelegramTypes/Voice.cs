using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Voice
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }
}
