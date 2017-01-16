using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class StreamsPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("streams")]
        public List<Stream> streams { get; protected set; }
    }
}
