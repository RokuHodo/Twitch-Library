//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class Emotes
    {
        [JsonProperty("emoticons")]
        public List<Emote> emoticon_sets { get; protected set; }
    }
}
