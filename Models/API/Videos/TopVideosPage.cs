// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class TopVideosPage
    {
        [JsonProperty("vods")]
        public List<Video> vods { get; protected set; }
    }
}
