// project namespaces
using TwitchLibrary.Models.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class NoticeEventArgs : IrcMessageEventArgs
    {
        public Notice notice_message { get; protected set; }

        public NoticeEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            notice_message = new Notice(_irc_message);
        }
    }
}
