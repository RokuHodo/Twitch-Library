//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class DeleteReactionResponse : HttpStatus
    {
        [JsonProperty("deleted")]
        public bool deleted { get; protected set; }
    }
}
