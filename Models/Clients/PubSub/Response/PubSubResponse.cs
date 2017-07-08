// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Response
{
    public class PubSubResponse
    {
        [JsonProperty("type")]
        public string type { get; internal set; }

        [JsonProperty("nonce")]
        public string nonce { get; internal set; }

        [JsonProperty("error")]
        public string error { get; internal set; }
    }
}
