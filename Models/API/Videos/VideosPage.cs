//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{    
    public class VideosPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("videos")]
        public List<Video> videos { get; protected set; }
    } 
}
