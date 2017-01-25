//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Games
{
    public class Game
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("box")]
        public Box box { get; protected set; }

        [JsonProperty("giantbomb_id")]
        public string giantbomb_id { get; protected set; }

        [JsonProperty("logo")]
        public Logo logo { get; protected set; }

        [JsonProperty("name")]
        public string name { get; protected set; }

        [JsonProperty("popularity")]
        public int popularity { get; protected set; }
    }
}
