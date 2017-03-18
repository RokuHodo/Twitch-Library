//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Games
{
    public class TopGamesPage
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("top")]
        public List<TopGame> top { get; protected set; }
    }
}
