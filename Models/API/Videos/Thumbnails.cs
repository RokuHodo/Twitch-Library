//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class Thumbnails
    {
        [JsonProperty("large")]
        public List<Thumbnail> large { get; protected set; }

        [JsonProperty("medium")]
        public List<Thumbnail> medium { get; protected set; }

        [JsonProperty("small")]
        public List<Thumbnail> small { get; protected set; }

        [JsonProperty("template")]
        public List<Thumbnail> template { get; protected set; }
    }
}
