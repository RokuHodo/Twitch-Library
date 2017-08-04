// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Commerce
{
    public class CommerceMessage
    {
        [JsonProperty("user_name")]
        public string user_name { get; internal set; }

        [JsonProperty("display_name")]
        public string display_name { get; internal set; }

        [JsonProperty("channel_name")]
        public string channel_name { get; internal set; }

        [JsonProperty("user_id")]
        public string user_id { get; internal set; }

        [JsonProperty("channel_id")]
        public string channel_id { get; internal set; }

        [JsonProperty("time")]
        public DateTime time { get; internal set; }

        [JsonProperty("item_image_url")]
        public string item_image_url { get; internal set; }

        [JsonProperty("item_description")]
        public string item_description { get; internal set; }

        [JsonProperty("supports_channel")]
        public bool supports_channel { get; internal set; }

        [JsonProperty("purchase_message")]
        public PurchaseMessage purchase_message { get; internal set; }
    }
}
