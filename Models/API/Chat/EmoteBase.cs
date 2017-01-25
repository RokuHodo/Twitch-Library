//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class EmoteBase
    {
        [JsonProperty("code")]
        public string code { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }
    }
}
