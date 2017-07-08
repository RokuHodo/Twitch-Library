// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class VideosFollowsPage
    {
        [JsonProperty("videos")]
        public List<Video> videos { get; protected set; }
    }
}
