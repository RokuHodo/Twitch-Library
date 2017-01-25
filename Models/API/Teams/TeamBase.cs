using System;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Teams
{
    public class TeamBase : HttpStatus
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("background")]
        public string background { get; protected set; }

        [JsonProperty("banner")]
        public string banner { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("display_name")]
        public string display_name { get; protected set; }

        [JsonProperty("info")]
        public string info { get; protected set; }

        [JsonProperty("logo")]
        public string logo { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }
    }
}