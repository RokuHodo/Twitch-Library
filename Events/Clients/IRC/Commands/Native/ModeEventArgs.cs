// project namespaces
using TwitchLibrary.Models.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Commands.Native;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Native
{
    public class ModeEventArgs : IrcMessageEventArgs
    {
        public Mode mode_message { get; protected set; }

        public ModeEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            mode_message = new Mode(_irc_message);
        }
    }
}
