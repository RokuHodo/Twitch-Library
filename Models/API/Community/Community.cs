// project namespaces
using TwitchLibrary.Enums.Helpers.Paging;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class Community
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("avatar_image_url")]
        public string avatar_image_url { get; protected set; }

        [JsonProperty("cover_image_url")]
        public string created_at { get; protected set; }

        [JsonProperty("description")]
        public string description { get; protected set; }

        [JsonProperty("description_html")]
        public string description_html { get; protected set; }

        // TODO: (Models) API - Community - re-implement StreamLanguage once I figure out how to properly handle the - to _ converison
        [JsonProperty("language")]
        public string language { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("owner_id")]
        public string owner_id { get; protected set; }

        [JsonProperty("rules")]
        public string rules { get; protected set; }

        [JsonProperty("rules_html")]
        public string rules_html { get; protected set; }

        [JsonProperty("summary")]
        public string summary { get; protected set; }
    }
}
