//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Channels;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class UserSubscriber : HttpStatus
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("channel")]
        public Channel user { get; protected set; }
    }
}
