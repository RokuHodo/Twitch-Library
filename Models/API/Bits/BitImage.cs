// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Bits
{
    public class BitImage
    {
        [JsonProperty("animated")]
        public Dictionary<string, string> animated_image { get; protected set; }

        [JsonProperty("static")]
        public Dictionary<string, string> static_image { get; protected set; }
    }
}
