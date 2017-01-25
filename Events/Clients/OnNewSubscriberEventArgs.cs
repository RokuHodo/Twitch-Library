using System;

using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients
{
    public class OnNewSubscriberEventArgs : EventArgs
    {
        public NewSubcriberMessage subscriber_message { get; set; }
    }
}
