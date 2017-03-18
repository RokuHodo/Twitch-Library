//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Users;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class ChannelSubscriber : HttpStatus
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("user")]
        public User user { get; protected set; }
    }
}
