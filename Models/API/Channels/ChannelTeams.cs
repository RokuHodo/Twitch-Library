//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;
using TwitchLibrary.Models.API.Teams;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class ChannelTeams : HttpStatus
    {
        [JsonProperty("teams")]
        public List<TeamBase> teams { get; protected set; }
    }
}
