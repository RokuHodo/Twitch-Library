
//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Games
{
    public class TopGame
    {
        [JsonProperty("channels")]
        public int channels { get; protected set; }

        [JsonProperty("viewers")]
        public int viewers { get; protected set; }

        [JsonProperty("game")]
        public Game game { get; protected set; }
    }
}
