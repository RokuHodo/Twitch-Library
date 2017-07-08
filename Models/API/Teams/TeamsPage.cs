// standard namespaces
using System.Collections.Generic;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Teams
{
    public class TeamsPage
    {
        [JsonProperty("teams")]
        public List<TeamBase> teams { get; protected set; }
    }
}
