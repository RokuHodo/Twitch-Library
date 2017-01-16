//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class StreamSummary
    {
        [JsonProperty("channels")]
        public int channels { get; protected set; }

        [JsonProperty("viewers")]
        public int viewers { get; protected set; }
    }
}
