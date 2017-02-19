using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Enums.Helpers.Paging;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Community
{
    public class CreatedCommunity
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }
    }
}
