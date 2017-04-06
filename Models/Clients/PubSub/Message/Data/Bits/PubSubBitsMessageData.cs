//standard namespaces
using System;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits
{
    public class PubSubBitsMessageData
    {
        [JsonProperty("user_name")]
        public string user_name { get; internal set; }

        [JsonProperty("channel_name")]
        public string channel_name { get; internal set; }

        [JsonProperty("user_id")]
        public string user_id { get; internal set; }

        [JsonProperty("channel_id")]
        public string channel_id { get; internal set; }

        [JsonProperty("time")]
        public DateTime time { get; internal set; }

        [JsonProperty("chat_message")]
        public string chat_message { get; internal set; }

        [JsonProperty("bits_used")]
        public int bits_used { get; internal set; }

        [JsonProperty("total_bits_used")]
        public int total_bits_used { get; internal set; }

        [JsonProperty("context")]
        public string context { get; internal set; }

        [JsonProperty("badge_entitlement")]
        public PubSubBitsBadgeEntitlement badge_entitlement { get; internal set; }
    }
}
