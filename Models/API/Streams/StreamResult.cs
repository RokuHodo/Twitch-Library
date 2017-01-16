//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class StreamResult
    {
        [JsonProperty("stream")]
        public Stream stream { get; protected set; }
    }
}
