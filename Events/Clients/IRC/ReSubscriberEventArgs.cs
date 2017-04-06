//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class UseResubscriberEventArgs : EventArgs
    {
        public ResubscriberMessage subscriber_message { get; set; }
    }
}
