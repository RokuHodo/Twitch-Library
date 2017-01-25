using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Clips
{
    public class ClipsPage
    {
        [JsonProperty("_cursor")]
        public string _cursor { get; protected set; }

        [JsonProperty("clips")]
        public List<Clip> clips { get; protected set; }
    }
}
