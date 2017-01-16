using System;

//project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Users;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class CreateReactionResponse : HttpStatus
    {
        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("emote_id")]
        public string emote_id { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("user")]
        public User user { get; protected set; }
    }
}
