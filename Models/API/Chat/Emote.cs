// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class Emote : EmoteBase
    {
        [JsonProperty("emoticon_set")]
        public string emoticon_set { get; protected set; }
    }
}
