//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class Vod
    {
        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }
    }
}
