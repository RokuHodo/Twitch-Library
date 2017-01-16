using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class FollowsPage : HttpStatus
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("follows")]
        public List<Follow> follows { get; protected set; }      
    }
}
