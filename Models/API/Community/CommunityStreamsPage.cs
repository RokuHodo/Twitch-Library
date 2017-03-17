using System.Collections.Generic;

using TwitchLibrary.Models.API.Streams;

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
