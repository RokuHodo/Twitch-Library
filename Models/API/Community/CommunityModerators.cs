using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Models.API.Users;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CommunityModerators
    {
        [JsonProperty("moderators")]
        public List<User> moderators { get; protected set; }
    }
}
