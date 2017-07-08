// project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC.Trailing;

// Example
// @msg-id=slow_on 
// :tmi.twitch.tv NOTICE #rokuhodo_ :This room is now in slow mode. You may send messages every 10 seconds.

namespace TwitchLibrary.Models.Messages.IRC.Commands.Twitch
{
    public class Notice : Body
    {
        public string msg_id        { get; protected set; }
        public string channel_name  { get; protected set; }

        public Notice(IrcMessage irc_message) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                msg_id = TagConverter.ToGeneric<string>(irc_message.tags, "msg-id");
            }

            channel_name = irc_message.middle[0].TextAfter("#");
        }
    }
}
