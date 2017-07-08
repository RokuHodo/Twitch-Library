// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Bits
{
    public class BitImages
    {
        [JsonProperty("dark")]
        public BitImage dark { get; protected set; }

        [JsonProperty("light")]
        public BitImage light { get; protected set; }
    }
}
