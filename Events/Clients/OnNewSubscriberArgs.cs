using System;

using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients
{
    public class OnNewSubscriberArgs : EventArgs
    {
        public NewSubcriberMessage subscriber_message { get; set; }
    }
}
