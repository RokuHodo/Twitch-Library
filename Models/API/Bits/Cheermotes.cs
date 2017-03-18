//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Bits
{
    public class Cheermotes
    {
        [JsonProperty("actions")]
        public List<Action> actions { get; protected set; }
    }
}
