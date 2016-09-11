using Newtonsoft.Json;

namespace Kondor.Data
{
    public abstract class JsonSerializable : IJsonSerializable
    {
        public string ToJson()
        {
            var result = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return result;
        }
    }
}
