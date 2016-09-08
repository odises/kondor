using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Location
    {
        [JsonProperty("longitude")]
        public float Longitude { get; set; }

        [JsonProperty("latitude")]
        public float Latitude { get; set; }
    }
}
