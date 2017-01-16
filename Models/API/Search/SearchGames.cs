using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.Games;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Search
{
    public class SearchGames
    {                
        [JsonProperty("games")]
        public List<Game> games { get; protected set; }        
    }
}
