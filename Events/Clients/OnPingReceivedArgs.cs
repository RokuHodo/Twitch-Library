using System;

namespace TwitchLibrary.Events.Clients
{
    public class OnPingReceivedArgs : EventArgs
    {
        public string ping_message { get; set; }
    }
}
