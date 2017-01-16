using System;
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.Channels;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class Video
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("broadcast_id")]
        public long broadcast_id { get; protected set; }

        [JsonProperty("broadcast_type")]
        public string broadcast_type { get; protected set; }

        [JsonProperty("channel")]
        public Channel channel { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("description")]
        public string description { get; protected set; }

        [JsonProperty("description_html")]
        public string description_html { get; protected set; }

        [JsonProperty("fps")]
        public Dictionary<string, double> fps { get; protected set; }               //varies by video type and channel 

        [JsonProperty("game")]
        public string game { get; protected set; }

        [JsonProperty("language")]
        public string language { get; protected set; }

        [JsonProperty("length")]
        public int length { get; protected set; }

        [JsonProperty("muted_segments")]
        public string muted_segments { get; protected set; }                        

        [JsonProperty("preview")]
        public Preview preview { get; protected set; }

        [JsonProperty("published_at")]
        public DateTime published_at { get; protected set; }

        [JsonProperty("resolutions")]
        public Dictionary<string, string> resolutions { get; protected set; }       //could be 1080p or chucked, etc

        [JsonProperty("status")]
        public string status { get; protected set; }

        [JsonProperty("tag_list")]
        public string tag_list { get; protected set; }

        [JsonProperty("thumbnails")]
        public Thumbnails thumbnails { get; protected set; }

        [JsonProperty("title")]
        public string title { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }

        [JsonProperty("viewable")]
        public string viewable { get; protected set; }

        [JsonProperty("viewable_at")]
        public string viewable_at { get; protected set; }

        [JsonProperty("views")]
        public int views { get; protected set; }
    }
}
