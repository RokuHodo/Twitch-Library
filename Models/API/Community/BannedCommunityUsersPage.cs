// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class BannedCommunityUsersPage
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("banned_users")]
        public List<BannedCommunityUser> banned_users { get; protected set; }
    }
}
