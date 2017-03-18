//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class PostsPage : HttpStatus
    {   
        [JsonProperty("_disabled")]
        public bool _disabled { get; protected set; }

        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("_topic")]
        public string _topic { get; protected set; }        

        [JsonProperty("posts")]
        public List<Post> posts { get; protected set; }
    }
}
