// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Channels;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class UserSubscriber : HttpStatus
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("sub_plan")]
        public string sub_plan { get; protected set; }

        [JsonProperty("sub_plan_name")]
        public string sub_plan_name { get; protected set; }

        [JsonProperty("channel")]
        public Channel channel { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }
    }
}
