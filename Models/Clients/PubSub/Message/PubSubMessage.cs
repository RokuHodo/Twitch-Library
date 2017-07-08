// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message
{
    public class PubSubMessage
    {
        [JsonProperty("type")]
        public string type { get; internal set; }

        [JsonProperty("data")]
        public PubSubMessageData data { get; internal set; }
    }
}
