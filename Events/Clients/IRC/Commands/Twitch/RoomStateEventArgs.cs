// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class RoomStateEventArgs : IrcMessageEventArgs
    {
        public RoomState room_state_message { get; protected set; }

        public RoomStateEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            room_state_message = new RoomState(_irc_message);
        }
    }
}
