// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Native;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Native
{
    public class PrivmsgEventArgs : IrcMessageEventArgs
    {
        public Privmsg private_message { get; protected set; }

        public PrivmsgEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            private_message = new Privmsg(_irc_message);
        }
    }
}
