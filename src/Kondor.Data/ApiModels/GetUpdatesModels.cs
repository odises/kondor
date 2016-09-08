using System.Collections.Generic;
using Kondor.Data.TelegramTypes;
using Newtonsoft.Json;

namespace Kondor.Data.ApiModels
{
    public class GetUpdatesResult
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public List<Update> Result { get; set; }
    }
}
