// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class BlockedUser
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }

        [JsonProperty("user")]
        public User user { get; protected set; }
    }
}
