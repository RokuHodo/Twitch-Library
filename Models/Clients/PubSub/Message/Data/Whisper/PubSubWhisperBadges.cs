//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper
{
    public class PubSubWhisperBadges
    {
        [JsonProperty("id")]
        public string id { get; internal set; }

        [JsonProperty("version")]
        public string version { get; internal set; }
    }
}
