// project namespaces
using TwitchLibrary.Models.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class UserStateEventArgs : IrcMessageEventArgs
    {
        public UserState user_state_message { get; protected set; }

        public UserStateEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            user_state_message = new UserState(_irc_message);
        }
    }
}
