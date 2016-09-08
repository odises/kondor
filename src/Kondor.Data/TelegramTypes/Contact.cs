using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Contact
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }
}
