using System;

//project namespaces
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class Stream : HttpStatus
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("community_id")]
        public string community_id { get; protected set; }

        [JsonProperty("game")]
        public string game { get; protected set; }

        [JsonProperty("viewers")]
        public int viewers { get; protected set; }

        [JsonProperty("video_height")]
        public int video_height { get; protected set; }

        [JsonProperty("average_fps")]
        public double average_fps { get; protected set; }

        [JsonProperty("delay")]
        public int delay { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("is_playlist")]
        public bool is_playlist { get; protected set; }

        [JsonProperty("preview")]
        public Preview preview { get; protected set; }

        [JsonProperty("channel")]
        public Channel channel { get; protected set; }     
    }
}
