using System;

//project namespaces
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class Follow : HttpStatus
    {
        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("notifications")]
        public bool notifications { get; protected set; }

        [JsonProperty("channel")]
        public Channel channel { get; protected set; }
    }
}
