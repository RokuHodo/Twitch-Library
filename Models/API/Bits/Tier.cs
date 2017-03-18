//standard namespaces
using System.Drawing;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Bits
{
    public class Tier
    {
        [JsonProperty("color")]
        public Color color { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("images")]
        public BitImages images { get; protected set; }

        [JsonProperty("min_bits")]
        public int min_bits { get; protected set; }
    }
}
