using System;

using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Events.Clients
{
    public class OnIrcMessageReceivedArgs : EventArgs
    {
        public IrcMessage irc_message { get; set; }
    }
}
