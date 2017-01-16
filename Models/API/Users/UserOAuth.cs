//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class UserOAuth : User
    {
        [JsonProperty("email")]
        public string email { get; protected set; }

        [JsonProperty("email_verified")]
        public bool email_verified { get; protected set; }

        [JsonProperty("notifications")]
        public Notifications notifications { get; protected set; }

        [JsonProperty("partnered")]
        public bool partnered { get; protected set; }

        [JsonProperty("twitter_connected")]
        public bool twitter_connected { get; protected set; }      
    }
}
