// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Users
{
    public class VHSConnectionCheck
    {
        [JsonProperty("identifier")]
        public string identifier { get; protected set; }
    }
}
