// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Ingests
{
    public class Ingest
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("availability")]
        public decimal availability { get; protected set; }

        [JsonProperty("default")]
        public bool _default { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("url_template")]
        public string url_template { get; protected set; }
    }
}
