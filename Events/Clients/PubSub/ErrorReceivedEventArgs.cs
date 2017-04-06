//standard namespaces
using System;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class ErrorReceivedEventArgs : EventArgs
    {
        public Exception exception { get; internal set; }
        public string message { get; internal set; }
    }
}
