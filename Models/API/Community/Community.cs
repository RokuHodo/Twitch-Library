﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

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

        [JsonProperty("language")]
        public StreamLanguage language { get; protected set; }

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
