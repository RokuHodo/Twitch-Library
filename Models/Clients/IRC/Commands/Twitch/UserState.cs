// standard namespaces
using System.Collections.Generic;
using System.Drawing;

// project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Clients.IRC.Tags;

// Example
// @badges=broadcaster/1,premium/1;
// color=#FF0000;
// display-name=RokuHodo_;
// emote-sets=0,33,140,168,1570,1630,2963,16595,19194,33563;
// mod=1;
// subscriber=0;
// user-type=mod 
// :tmi.twitch.tv USERSTATE #rokuhodo_

namespace TwitchLibrary.Models.Clients.IRC.Commands.Twitch
{
    public class UserState
    {
        public int          mod             { get; protected set; }
        public int          subscriber      { get; protected set; }

        public string       display_name    { get; protected set; }
        public string       channel_name    { get; protected set; }

        public List<string> emote_sets      { get; protected set; }

        public List<Badge>  badges          { get; protected set; }

        public UserType     user_type       { get; protected set; }

        public Color        color           { get; protected set; }

        public UserState(IrcMessage irc_message)
        {
            if (irc_message.contains_tags)
            {
                mod             = TagConverter.ToGeneric<int>(irc_message.tags, "mod");
                subscriber      = TagConverter.ToGeneric<int>(irc_message.tags, "subscriber");

                display_name    = TagConverter.ToGeneric<string>(irc_message.tags, "display-name");

                emote_sets      = TagConverter.ToList<string>(irc_message.tags, "emote-sets");

                badges          = TagConverter.ToBadges(irc_message.tags, "badges");

                user_type       = TagConverter.ToUserType(irc_message.tags, "user-type", badges);

                color           = TagConverter.ToColor(irc_message.tags, "color");
            }

            channel_name = irc_message.middle[0].TextAfter("#");
        }
    }
}
