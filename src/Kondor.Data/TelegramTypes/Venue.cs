using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Venue
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("foursquare_id")]
        public string FoursquareId { get; set; }
    }
}
