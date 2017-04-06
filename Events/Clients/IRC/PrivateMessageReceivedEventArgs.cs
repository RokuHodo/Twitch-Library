//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Private;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class PrivateMessageReceivedEventArgs : EventArgs
    {
        public PrivateMessage private_message { get; set; }
    }
}
