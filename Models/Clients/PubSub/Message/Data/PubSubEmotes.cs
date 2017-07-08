// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data
{
    public class PubSubEmotes
    {
        [JsonProperty("id")]
        public string id { get; internal set; }

        [JsonProperty("start")]
        public int start { get; internal set; }

        [JsonProperty("end")]
        public int end { get; internal set; }
    }
}
