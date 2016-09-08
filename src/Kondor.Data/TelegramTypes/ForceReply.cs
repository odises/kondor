using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class ForceReply
    {
        [JsonProperty("force_reply")]
        public bool ForceReplyAttribute => true;

        [JsonProperty("selective")]
        public bool Selective { get; set; }
    }
}