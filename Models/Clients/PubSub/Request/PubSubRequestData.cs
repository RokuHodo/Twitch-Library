// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Request
{
    public class PubSubRequestData
    {
        [JsonProperty("topics")]
        public List<string> topics { get; internal set; }

        [JsonProperty("auth_token")]
        public string auth_token { get; internal set; }
    }
}
