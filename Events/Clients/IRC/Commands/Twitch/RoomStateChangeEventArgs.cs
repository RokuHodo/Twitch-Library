// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class RoomStateChangeEventArgs : IrcMessageEventArgs
    {
        public RoomStateChange room_state_change_message { get; protected set; }

        public RoomStateChangeEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            room_state_change_message = new RoomStateChange(_irc_message);
        }
    }
}
