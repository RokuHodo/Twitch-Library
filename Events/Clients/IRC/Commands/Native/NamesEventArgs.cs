// standard namespaces
using TwitchLibrary.Models.Clients.IRC;
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Models.Clients.IRC.Commands.Native;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Native
{
    public class NamesEventArgs : IrcMessageEventArgs
    {
        public Names names_message { get; protected set; }

        public NamesEventArgs(string _raw_message, IrcMessage _irc_message, List<string> _names) : base(_raw_message, _irc_message)
        {
            names_message = new Names(_irc_message, _names);
        }
    }
}
