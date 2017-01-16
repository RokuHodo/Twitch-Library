using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class EmoteImages
    {
        [JsonProperty("_links")]
        public Dictionary<string, string> _links { get; set; }        

        [JsonProperty("emoticons")]
        public List<EmoteImage> emoticons { get; set; }
    }
}
