
//project namespaces

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Videos
{
    public class CreatedVideo
    {
        [JsonProperty("upload")]
        public Upload upload { get; protected set; }

        [JsonProperty("video")]
        public Video video { get; protected set; }
    }
}
