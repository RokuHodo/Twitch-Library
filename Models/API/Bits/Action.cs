//standard namespaces
using System;
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Bits
{
    public class Action
    {
        [JsonProperty("backgrounds")]
        public List<string> backgrounds { get; protected set; }

        [JsonProperty("prefix")]
        public string prefix { get; protected set; }

        [JsonProperty("scales")]
        public List<string> scales { get; protected set; }

        [JsonProperty("states")]
        public List<string> states { get; protected set; }

        [JsonProperty("tiers")]
        public List<Tier> tiers { get; protected set; }

        [JsonProperty("type")]
        public string type { get; protected set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; protected set; }
    }
}
