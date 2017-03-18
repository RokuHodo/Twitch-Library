//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class ClipChannel
    {
        [JsonProperty("channel_url")]
        public string channel_url { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("logo")]
        public string logo { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("display_name")]
        public string display_name { get; protected set; }
    }
}
