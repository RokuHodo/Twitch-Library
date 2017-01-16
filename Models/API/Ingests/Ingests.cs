using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Ingests
{
    public class Ingests
    {
        [JsonProperty("ingests")]
        public List<Ingest> ingests { get; protected set; }
    }
}
