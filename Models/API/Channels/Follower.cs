//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class Follower : HttpStatus
    {
        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("notifications")]
        public bool notifications { get; protected set; }

        [JsonProperty("user")]
        public User user { get; protected set; }
    }
}
