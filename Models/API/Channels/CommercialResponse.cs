//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class CommercialResponse : HttpStatus
    {
        [JsonProperty("duration")]
        public int duration { get; protected set; }

        [JsonProperty("message")]
        public string message { get; protected set; }

        [JsonProperty("retryafter")]
        public int retryafter { get; protected set; }
    }
}
