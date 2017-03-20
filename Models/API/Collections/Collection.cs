//standard namespaces
using System.Collections.Generic;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Collections
{
    public class Collection
    {
        [JsonProperty("_id")]
        public string _id { get; protected set; }

        [JsonProperty("items")]
        public List<Item> items { get; protected set; }
    }
}
