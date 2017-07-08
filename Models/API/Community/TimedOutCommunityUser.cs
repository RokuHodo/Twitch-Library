// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class TimedOutCommunityUser : BannedCommunityUser
    {
        [JsonProperty("ended_at")]
        public DateTime ended_at { get; protected set; }
    }
}
