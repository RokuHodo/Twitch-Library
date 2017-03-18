//standard namespaces
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Models.API.HTTP;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.API.Channels
{
    public class ChannelSubscribersPage : HttpStatus
    {
        [JsonProperty("_total")]
        public int _total { get; protected set; }

        [JsonProperty("subscriptions")]
        public List<ChannelSubscriber> subscriptions { get; protected set; }
    }
}
