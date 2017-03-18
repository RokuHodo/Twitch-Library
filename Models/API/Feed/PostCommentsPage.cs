//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Feed
{
    public class PostCommentsPage : HttpStatus
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("comments")]
        public List<Comment> comments { get; protected set; }
    }
}
