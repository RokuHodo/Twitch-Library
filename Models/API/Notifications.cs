//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API
{
    public class Notifications
    {
        [JsonProperty("email")]
        public bool email { get; protected set; }

        [JsonProperty("push")]
        public bool push { get; protected set; }
    }
}
