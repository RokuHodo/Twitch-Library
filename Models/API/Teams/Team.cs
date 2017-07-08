// standard namespaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Models.API.Channels;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Teams
{
    public class Team : TeamBase
    {
        [JsonProperty("users")]
        public List<Channel> users { get; protected set; }
    }
}
