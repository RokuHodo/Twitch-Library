// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class Upload
    {
        [JsonProperty("token")]
        public string token { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }
    }
}
