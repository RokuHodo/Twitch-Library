//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Chat
{
    public class EmoteImage
    {
        [JsonProperty("regex")]
        public string regex { get; protected set; }

        [JsonProperty("images")]
        public List<Image> images { get; protected set; }
    }
}
