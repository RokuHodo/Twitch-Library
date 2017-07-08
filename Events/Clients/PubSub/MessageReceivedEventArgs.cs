// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubMessage message { get; internal set; }
    }
}
