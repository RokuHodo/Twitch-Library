using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class EmoteSet
    {
        [JsonProperty("emoticon_sets")]
        public Dictionary<int, List<EmoteBase>> emoticon_sets { get; protected set; }
    }
}
