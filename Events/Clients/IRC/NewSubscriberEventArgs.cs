//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Subscriber;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class UserSubscribedEventArgs : EventArgs
    {
        public UserSubcribedMessage subscriber_message { get; set; }
    }
}
