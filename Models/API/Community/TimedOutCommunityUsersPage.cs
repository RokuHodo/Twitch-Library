//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class TimedOutCommunityUsersPage
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("timed_out_users")]
        public List<TimedOutCommunityUser> timed_out_users { get; protected set; }
    }
}
