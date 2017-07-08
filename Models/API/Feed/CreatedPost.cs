// project namespaces
using TwitchLibrary.Models.API.HTTP;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class CreatedPost : HttpStatus
    {
        [JsonProperty("tweet")]
        public string tweet { get; protected set; }

        [JsonProperty("post")]
        public Post post { get; protected set; }
    }
}
