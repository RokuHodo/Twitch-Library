using System;

using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients
{
    public class OnReSubscriberArgs : EventArgs
    {
        public ReSubscriberMessage subscriber_message { get; set; }
    }
}
