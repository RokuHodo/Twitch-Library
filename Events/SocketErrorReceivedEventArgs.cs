// standard namespaces
using System;
using System.Net.Sockets;

namespace TwitchLibrary.Events.Clients
{
    public class SocketErrorReceivedEventArgs : EventArgs
    {
        public SocketError error { get; internal set; }
    }
}
