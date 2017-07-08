// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.HTTP
{
    public class HttpStatus
    {
        [JsonProperty("error")]
        public string status_error { get; protected set; }

        [JsonProperty("message")]
        public string status_message { get; protected set; }

        [JsonProperty("status")]
        public int status_code { get; protected set; }
    }    
}
