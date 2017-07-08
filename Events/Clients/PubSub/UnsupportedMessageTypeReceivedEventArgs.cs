// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Enums.Clients.PubSub;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class UnsupportedMessageTypeReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubType type { get; internal set; }
    }
}
