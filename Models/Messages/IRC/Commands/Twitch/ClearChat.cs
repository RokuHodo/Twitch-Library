// project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;

// Example
// @ban-duration=10;
// ban-reason=;
// room-id=45947671;
// target-user-id=51012150
// :tmi.twitch.tv CLEARCHAT #rokuhodo_ :rokubotto

namespace TwitchLibrary.Models.Messages.IRC.Commands.Twitch
{
    public class ClearChat
    {
        public int      ban_duration        { get; protected set; }

        public string   ban_reason          { get; protected set; }
        public string   room_id             { get; protected set; }
        public string   room_name           { get; protected set; }
        public string   target_user_id      { get; protected set; }
        public string   target_user_name    { get; protected set; }

        public ClearChat(IrcMessage irc_message)
        {
            if (irc_message.contains_tags)
            {
                ban_duration =      TagConverter.ToGeneric<int>(irc_message.tags, "ban-duration");

                ban_reason =        TagConverter.ToGeneric<string>(irc_message.tags, "ban-reason");
                room_id =           TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                target_user_id =    TagConverter.ToGeneric<string>(irc_message.tags, "target-user-id");
            }

            room_name = irc_message.middle[0].TextAfter('#');

            // there is no target user if the entire chat is cleared
            if (irc_message.trailing.isValid())
            {
                target_user_name = irc_message.trailing[0];
            }
        }
    }
}
