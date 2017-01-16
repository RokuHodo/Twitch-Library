//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class Image
    {
        [JsonProperty("width")]
        public int width { get; protected set; }

        [JsonProperty("height")]
        public int height { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }

        [JsonProperty("emoticon_set")]
        public int emoticon_set { get; protected set; }
    }
}
