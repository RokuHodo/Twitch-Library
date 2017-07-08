﻿// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API
{
    public class Preview
    {
        [JsonProperty("small")]
        public string small { get; protected set; }

        [JsonProperty("medium")]
        public string medium { get; protected set; }

        [JsonProperty("large")]
        public string large { get; protected set; }

        [JsonProperty("template")]
        public string template { get; protected set; }
    }
}
