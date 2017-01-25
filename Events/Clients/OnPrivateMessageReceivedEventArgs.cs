using System;

using TwitchLibrary.Models.Messages.Private;

namespace TwitchLibrary.Events.Clients
{
    public class OnPrivateMessageReceivedEventArgs : EventArgs
    {
        public PrivateMessage private_message { get; set; }
    }
}
