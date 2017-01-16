using System;

namespace TwitchLibrary.Events.Clients
{
    public class OnNullIrcMessageReceivedArgs : EventArgs
    {        
        public DateTime time { get; set; }
    }
}
