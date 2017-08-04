// project namespaces
using TwitchLibrary.Models.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class GlobalUserStateEventArgs : IrcMessageEventArgs
    {
        public GlobalUserState global_user_state_message { get; protected set; }

        public GlobalUserStateEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            global_user_state_message = new GlobalUserState(_irc_message);
        }
    }
}
