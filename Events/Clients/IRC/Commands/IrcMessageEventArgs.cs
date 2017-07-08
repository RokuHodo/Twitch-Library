// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class IrcMessageEventArgs : EventArgs
    {
        public string       raw_message { get; protected set; }

        public IrcMessage   irc_message { get; protected set; }

        public IrcMessageEventArgs(string _raw_message, IrcMessage _irc_message)
        {
            raw_message = _raw_message;

            irc_message = _irc_message;
        }
    }
}
