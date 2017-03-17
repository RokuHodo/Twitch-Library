
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CreatedCommunity
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }
    }
}
