//standard namespaces
using System;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Collections
{
    public class CollectionMetadata
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("items_count")]
        public int items_count { get; protected set; }

        [JsonProperty("owner")]
        public Owner owner { get; protected set; }

        [JsonProperty("thumbnails")]
        public Preview thumbnails { get; protected set; }

        [JsonProperty("title")]
        public string title { get; protected set; }

        [JsonProperty("total_duration")]
        public long total_duration { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }

        [JsonProperty("views")]
        public int views { get; protected set; }
    }
}
