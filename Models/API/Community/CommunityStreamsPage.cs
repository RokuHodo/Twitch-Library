// standard namespaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Models.API.Streams;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CommunityStreamsPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("streams")]
        public List<Stream> streams { get; protected set; }
    }
}
