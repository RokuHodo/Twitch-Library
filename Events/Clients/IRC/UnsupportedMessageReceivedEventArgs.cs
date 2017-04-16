//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class UnsupportedMessageReceivedEventArgs : EventArgs
    {
        public string message { get; internal set; }
        public IrcMessage irc_message { get; internal set; }
    }
}
