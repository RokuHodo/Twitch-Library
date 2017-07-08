// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class ChannelOAuth : Channel
    {
        [JsonProperty("email")]
        public string email { get; protected set; }

        [JsonProperty("stream_key")]
        public string stream_key { get; protected set; }
    }
}
