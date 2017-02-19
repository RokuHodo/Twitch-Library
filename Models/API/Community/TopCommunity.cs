using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class TopCommunity
    {
        [JsonProperty("avatar_image_url")]
        public string broadcaster_language { get; protected set; }

        [JsonProperty("channels")]
        public int channels { get; protected set; }

        [JsonProperty("_id")]
        public string _id { get; protected set; }                       

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("viewers")]
        public int viewers { get; protected set; }
    }
}
