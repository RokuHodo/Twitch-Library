using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class Vod
    {
        [JsonProperty("id")]
        public string id { get; protected set; }

        [JsonProperty("url")]
        public string url { get; protected set; }
    }
}
