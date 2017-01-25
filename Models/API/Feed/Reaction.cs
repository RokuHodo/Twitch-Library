using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class Reaction
    {
        [JsonProperty("count")]
        public int count { get; protected set; }

        [JsonProperty("emote")]
        public string emote { get; protected set; }

        [JsonProperty("user_ids")]
        public List<string> user_ids { get; protected set; }
    }
}
