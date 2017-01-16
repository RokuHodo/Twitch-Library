//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.HTTP
{
    public class HttpStatus
    {
        [JsonProperty("error")]
        public string http_error { get; protected set; }

        [JsonProperty("status")]
        public int http_status { get; protected set; }

        [JsonProperty("message")]
        public string http_message { get; protected set; }
    }    
}
