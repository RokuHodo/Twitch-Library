// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class UserNoticeEventArgs : IrcMessageEventArgs
    {
        public UserNotice user_notice_message { get; protected set; }

        public UserNoticeEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            user_notice_message = new UserNotice(_irc_message);
        }
    }
}
