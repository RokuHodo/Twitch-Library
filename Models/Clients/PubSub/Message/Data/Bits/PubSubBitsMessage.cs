//standard namespaces
using System;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits
{
    public class PubSubBitsMessage
    {
        [JsonProperty("data")]
        public PubSubBitsMessageData data { get; internal set; }
        
        [JsonProperty("version")]
        public string version { get; internal set; }

        [JsonProperty("message_type")]
        public string message_type { get; internal set; }

        [JsonProperty("message_id")]
        public string message_id { get; internal set; }
    }
}
