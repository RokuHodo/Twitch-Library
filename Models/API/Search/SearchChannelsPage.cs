//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.Channels;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Search
{
    public class SearchChannelsPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("channels")]
        public List<Channel> channels { get; protected set; }
    }
}
