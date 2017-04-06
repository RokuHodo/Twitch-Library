//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message
{
    public class PubSubMessageType
    {
        [JsonProperty("type")]
        public string type { get; internal set; }
    }
}
