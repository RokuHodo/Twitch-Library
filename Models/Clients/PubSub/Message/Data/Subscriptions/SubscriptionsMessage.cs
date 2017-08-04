using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Subscriptions
{
    public class SubscriptionsMessage
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

        [JsonProperty("sub_plan")]
        public string sub_plan { get; internal set; }

        [JsonProperty("sub_plan_name")]
        public string sub_plan_name { get; internal set; }

        [JsonProperty("months")]
        public int months { get; internal set; }

        [JsonProperty("context")]
        public string context { get; internal set; }

        [JsonProperty("sub_message")]
        public SubMessage sub_message { get; internal set; }
    }
}
