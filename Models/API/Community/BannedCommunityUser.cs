using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class BannedCommunityUser
    {
        [JsonProperty("user_id")]
        public string user_id { get; protected set; }

        [JsonProperty("display_name")]
        public string display_name { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("bio")]
        public string bio { get; protected set; }

        [JsonProperty("avatar_image_url")]
        public string avatar_image_url { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; protected set; }
    }
}
