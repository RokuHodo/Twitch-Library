//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class Permissions
    {
        [JsonProperty("can_delete")]
        public bool can_delete { get; protected set; }

        [JsonProperty("can_moderate")]
        public bool can_moderate { get; protected set; }

        [JsonProperty("can_reply")]
        public bool can_reply { get; protected set; }
    }
}
