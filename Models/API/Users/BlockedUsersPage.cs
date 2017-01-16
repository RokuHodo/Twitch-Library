using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class BlockedUsersPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("blocks")]
        public List<BlockedUser> blocks { get; protected set; }
    }
}
