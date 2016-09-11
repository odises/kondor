using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kondor.Data.ApiModels
{
    public class TelegramApiResponseModel
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
