//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class Badges
    {
        [JsonProperty("admin")]
        public Badge admin { get; protected set; }

        [JsonProperty("broadcaster")]
        public Badge broadcaster { get; protected set; }

        [JsonProperty("global_mod")]
        public Badge global_mod { get; protected set; }

        [JsonProperty("mod")]
        public Badge mod { get; protected set; }

        [JsonProperty("staff")]
        public Badge staff { get; protected set; }

        [JsonProperty("subscriber")]
        public Badge subscriber { get; protected set; }

        [JsonProperty("turbo")]
        public Badge turbo { get; protected set; }

        //not implemented by Twitch yet, only a matter of time
        [JsonProperty("premium")]
        public Badge premium { get; protected set; }
    }
}
