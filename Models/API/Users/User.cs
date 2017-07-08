// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class User
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("bio")]
        public string bio { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("display_name")]
        public string display_name { get; protected set; }

        [JsonProperty("logo")]
        public string logo { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("type")]
        public string type { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }       
    }
}
