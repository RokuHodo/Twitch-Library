using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Subscriptions
{
    public class PubSubSubscriptionsSubMessage
    {
        [JsonProperty("message")]
        public string message { get; internal set; }

        [JsonProperty("emotes")]
        public List<PubSubEmotes> emotes { get; internal set; }
    }
}
