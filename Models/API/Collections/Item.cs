// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Collections
{
    public class Item
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("description_html")]
        public string description_html { get; protected set; }

        [JsonProperty("duration")]
        public long duration { get; protected set; }

        [JsonProperty("game")]
        public string game { get; protected set; }

        [JsonProperty("item_id")]
        public string item_id { get; protected set; }

        [JsonProperty("item_type")]
        public string item_type { get; protected set; }

        [JsonProperty("owner")]
        public Owner owner { get; protected set; }

        [JsonProperty("published_at")]
        public DateTime published_at { get; protected set; }

        [JsonProperty("thumbnails")]
        public Preview thumbnails { get; protected set; }

        [JsonProperty("title")]
        public string title { get; protected set; }

        [JsonProperty("views")]
        public int views { get; protected set; }
    }
}
