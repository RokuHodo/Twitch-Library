// standard namespaces
using System;

namespace TwitchLibrary.Events.Clients
{
    public class ErrorReceivedEventArgs : EventArgs
    {
        public Exception exception { get; internal set; }
    }
}
