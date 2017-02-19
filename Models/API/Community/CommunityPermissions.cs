using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CommunityPermissions
    {
        [JsonProperty("ban")]
        public bool ban { get; protected set; }

        [JsonProperty("timeout")]
        public bool timeout { get; protected set; }

        [JsonProperty("edit")]
        public bool edit { get; protected set; }
    }
}
