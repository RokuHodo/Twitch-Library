using System;

using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients
{
    public class OnReSubscriberEventArgs : EventArgs
    {
        public ReSubscriberMessage subscriber_message { get; set; }
    }
}
