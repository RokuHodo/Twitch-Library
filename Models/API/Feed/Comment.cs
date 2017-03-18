//standard namespaces
using System;
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Users;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class Comment : HttpStatus
    {
        [JsonProperty("body")]
        public string body { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("deleted")]
        public bool deleted { get; protected set; }

        [JsonProperty("emotes")]
        public List<FeedEmote> emotes { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("permissions")]
        public Permissions permissions { get; protected set; }

        [JsonProperty("reactions")]
        public Dictionary<string, Reaction> reactions { get; protected set; }

        [JsonProperty("user")]
        public User user { get; protected set; }
    }
}
