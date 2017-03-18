//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Streams
{
    public class FeaturedStreamsPage : HttpStatus
    {
        [JsonProperty("featured")]
        public List<FeaturedStream> featured { get; protected set; }
    }
}
