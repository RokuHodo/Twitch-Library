// standard namespaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Models.API.Users;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CommunityModerators
    {
        [JsonProperty("moderators")]
        public List<User> moderators { get; protected set; }
    }
}
