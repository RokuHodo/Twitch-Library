//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class FollowerPage : HttpStatus
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("follows")]
        public List<Follower> follows { get; protected set; }              
    }
}
