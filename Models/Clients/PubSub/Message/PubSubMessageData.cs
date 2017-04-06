//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message
{
    public class PubSubMessageData
    {
        [JsonProperty("topic")]
        public string topic { get; internal set; }

        [JsonProperty("message")]
        public string message { get; internal set; }
    }
}
