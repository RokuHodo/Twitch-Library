using System.Collections.Generic;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class TopCommunityPage
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("communities")]
        public List<TopCommunity> communities { get; protected set; }
    }
}
