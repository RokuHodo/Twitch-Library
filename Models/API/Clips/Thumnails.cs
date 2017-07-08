// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class Thumbnails
    {
        [JsonProperty("medium")]
        public string medium { get; protected set; }

        [JsonProperty("small")]
        public string small { get; protected set; }

        [JsonProperty("tiny")]
        public string tiny { get; protected set; }
    }
}
