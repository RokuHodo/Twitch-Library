// project namespaces
using TwitchLibrary.Models.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Commands.Native;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Native
{
    public class JoinEventArgs : IrcMessageEventArgs
    {
        public Join join_message { get; protected set; }

        public JoinEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            join_message = new Join(_irc_message);
        }
    }
}
