// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class ClearChatEventArgs : IrcMessageEventArgs
    {
        public ClearChat clear_chat_message { get; protected set; }

        public ClearChatEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            clear_chat_message = new ClearChat(_irc_message);
        }
    }
}
