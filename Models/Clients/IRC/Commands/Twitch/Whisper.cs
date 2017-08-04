using System.Collections.Generic;
using System.Drawing;

// project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Clients.IRC.Tags;
using TwitchLibrary.Models.Clients.IRC.Trailing;

/*
// WHISPER Example
// @badges=premium/1;
// color=#FF0000;
// display-name=RokuHodo_;
// emotes=;
// message-id=561;
// thread-id=45947671_51012150;
// turbo=1;
// user-id=45947671;
// user-type= 
// :rokuhodo_!rokuhodo_@rokuhodo_.tmi.twitch.tv WHISPER rokubotto :test
*/
namespace TwitchLibrary.Models.Clients.IRC.Commands.Twitch
{
    public class Whisper : Body
    {
        public int          turbo           { get; protected set; }

        public string       display_name    { get; protected set; }
        public string       message_id      { get; protected set; }
        public string       thread_id       { get; protected set; }
        public string       recipient_id    { get; protected set; }
        public string       recipient_name  { get; protected set; }
        public string       user_id         { get; protected set; }
        public string       user_name       { get; protected set; }

        public List<Badge>  badges          { get; protected set; }

        public List<Emote>  emotes          { get; protected set; }    

        public UserType     user_type       { get; protected set; }

        public Color        color           { get; protected set; }

        public Whisper(IrcMessage irc_message) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                turbo           = TagConverter.ToGeneric<int>(irc_message.tags, "turbo");

                display_name    = TagConverter.ToGeneric<string>(irc_message.tags, "display-name");
                message_id      = TagConverter.ToGeneric<string>(irc_message.tags, "message-id");
                thread_id       = TagConverter.ToGeneric<string>(irc_message.tags, "thread-id");
                recipient_id    =  thread_id.TextAfter("_");
                user_id         = TagConverter.ToGeneric<string>(irc_message.tags, "user-id");

                badges          = TagConverter.ToBadges(irc_message.tags, "badges");

                emotes          = TagConverter.ToEmotes(irc_message.tags, "emotes");

                user_type       = TagConverter.ToUserType(irc_message.tags, "user-type", badges);

                color           = TagConverter.ToColor(irc_message.tags, "color");
            }

            recipient_name  = irc_message.middle[0];
            user_name       = irc_message.prefix.TextBetween('!', '@');
        }      
    }
}
