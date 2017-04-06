//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Clients.PubSub.Response;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class ResponseReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubResponse response { get; internal set; }
    }
}
