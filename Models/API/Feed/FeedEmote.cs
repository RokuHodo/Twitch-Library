//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class FeedEmote
    {
        [JsonProperty("end")]
        public int end { get; protected set; }

        [JsonProperty("id")]
        public int id { get; protected set; }

        [JsonProperty("set")]
        public int set { get; protected set; }

        [JsonProperty("start")]
        public int start { get; protected set; }
    }
}
