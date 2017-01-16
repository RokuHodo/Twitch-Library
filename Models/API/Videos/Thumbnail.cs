//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class Thumbnail
    {
        [JsonProperty("type")]
        public string type { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }
    }
}
