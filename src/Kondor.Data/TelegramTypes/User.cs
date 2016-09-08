using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
