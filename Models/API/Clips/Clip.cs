using System;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class Clip
    {
        [JsonProperty("broadcaster")]
        public ClipChannel broadcaster { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("curator")]
        public ClipChannel curator { get; protected set; }

        [JsonProperty("duration")]
        public double duration { get; protected set; }

        [JsonProperty("embed_html")]
        public string embed_html { get; protected set; }

        [JsonProperty("embed_url")]
        public string embed_url { get; protected set; }

        [JsonProperty("game")]
        public string game { get; protected set; }

        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("thumbnails")]
        public Thumbnails thumbnails { get; protected set; }

        [JsonProperty("title")]
        public string title { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }

        [JsonProperty("views")]
        public int views { get; protected set; }

        [JsonProperty("vod")]
        public Vod vod { get; protected set; }
    }
}
