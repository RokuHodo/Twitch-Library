//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Request
{
    public class PubSubRequest
    {
        [JsonProperty("type")]
        public string type { get; internal set; }

        [JsonProperty("nonce")]
        public string nonce { get; internal set; }

        [JsonProperty("data")]
        public PubSubRequestData data { get; internal set; }
    }
}
