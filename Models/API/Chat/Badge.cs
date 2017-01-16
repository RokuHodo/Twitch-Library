//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class Badge
    {
        [JsonProperty("alpha")]
        public string alpha { get; protected set; }

        [JsonProperty("image")]
        public string image { get; protected set; }

        [JsonProperty("svg")]
        public string svg { get; protected set; }
    }
}
