using System;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class Channel : HttpStatus
    {
        [JsonProperty("_id")]
        public int _id { get; protected set; }

        [JsonProperty("broadcaster_language")]
        public string broadcaster_language { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }

        [JsonProperty("display_name")]
        public string display_name { get; protected set; }

        [JsonProperty("followers")]
        public int followers { get; protected set; }

        [JsonProperty("game")]
        public string game { get; protected set; }

        [JsonProperty("language")]
        public string language { get; protected set; }

        [JsonProperty("logo")]
        public string logo { get; protected set; }

        [JsonProperty("mature")]
        public bool mature { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("partner")]
        public bool partner { get; protected set; }

        [JsonProperty("profile_banner")]
        public string profile_banner { get; protected set; }

        [JsonProperty("profile_banner_background_color")]
        public string profile_banner_background_color { get; protected set; }

        [JsonProperty("status")]
        public string status { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }

        [JsonProperty("video_banner")]
        public string video_banner { get; protected set; }

        [JsonProperty("views")]
        public int views { get; protected set; } 
    }
}
