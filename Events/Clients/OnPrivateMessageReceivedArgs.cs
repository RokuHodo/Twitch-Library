using System;

using TwitchLibrary.Models.Messages.Private;

namespace TwitchLibrary.Events.Clients
{
    public class OnPrivateMessageReceivedArgs : EventArgs
    {
        public PrivateMessage private_message { get; set; }
    }
}
