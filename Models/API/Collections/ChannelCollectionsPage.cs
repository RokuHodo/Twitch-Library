// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Collections
{
    public class ChannelCollectionsPage
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("collections")]
        public List<CollectionMetadata> collections { get; protected set; }
    }
}
