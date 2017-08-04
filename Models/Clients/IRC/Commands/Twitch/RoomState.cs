// project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;

// Example
// @broadcaster-lang=;emote-only=0;followers-only=-1;r9k=0;room-id=45947671;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #rokuhodo_
namespace TwitchLibrary.Models.Clients.IRC.Commands.Twitch
{
    public class RoomState
    {
        public int                  emote_only              { get; protected set; }
        public int                  followers_only          { get; protected set; }
        public int                  r9k                     { get; protected set; }
        public int                  slow                    { get; protected set; }
        public int                  sub_only                { get; protected set; }

        public string               room_name               { get; protected set; }
        public string               room_id                 { get; protected set; }

        public BroadcasterLanguage  broadcaster_language    { get; protected set; }

        public RoomState(IrcMessage irc_message)
        {
            if (irc_message.contains_tags)
            {
                emote_only              = TagConverter.ToGeneric<int>(irc_message.tags, "emote-only");
                followers_only          = TagConverter.ToGeneric<int>(irc_message.tags, "followers-only");
                r9k                     = TagConverter.ToGeneric<int>(irc_message.tags, "r9k");
                slow                    = TagConverter.ToGeneric<int>(irc_message.tags, "slow");
                sub_only                = TagConverter.ToGeneric<int>(irc_message.tags, "sub-only");

                room_id                 = TagConverter.ToGeneric<string>(irc_message.tags, "room-id");

                broadcaster_language    = TagConverter.ToEnum<BroadcasterLanguage>(irc_message.tags, "broadcaster-lang");
            }

            room_name = irc_message.middle[0].TextAfter('#');
        }
    }
}