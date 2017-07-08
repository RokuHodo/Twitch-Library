// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class FeaturedStream
    {
        [JsonProperty("image")]
        public string image { get; protected set; }

        [JsonProperty("priority")]
        public int priority { get; protected set; }

        [JsonProperty("scheduled")]
        public bool scheduled { get; protected set; }

        [JsonProperty("sponsored")]
        public bool sponsored { get; protected set; }

        [JsonProperty("stream")]
        public Stream stream { get; protected set; }

        [JsonProperty("text")]
        public string text { get; protected set; }

        [JsonProperty("title")]
        public string title { get; protected set; }
    }
}
