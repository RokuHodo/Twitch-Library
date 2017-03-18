//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients
{
    public class OnReSubscriberEventArgs : EventArgs
    {
        public ReSubscriberMessage subscriber_message { get; set; }
    }
}
