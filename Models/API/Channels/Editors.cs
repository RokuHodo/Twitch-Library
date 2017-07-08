// standard namespaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Users;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class Editors : HttpStatus
    {
        [JsonProperty("users")]
        public List<User> users { get; protected set; }     
    }
}
