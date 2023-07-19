using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClientStatus
    {
        Active,
        Done,
        Onhold
    }
}
