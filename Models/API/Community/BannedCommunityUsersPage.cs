using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

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
