// standard namespaces
using System;
using System.Collections.Generic;
using System.Drawing;

// project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC.Tags;
using TwitchLibrary.Models.Messages.IRC.Trailing;

// Example
/*
// badges=broadcaster/1,premium/1;
// bits=1000;
// color=#FF0000;
// display-name=RokuHodo_;
// emotes=;
// id=37607eff-1761-4dc7-99ea-b476277497e6;
// mod=1;
// room-id=45947671;
// sent-ts=1497390276217;
// subscriber=0;
// tmi-sent-ts=1497390277408;
// turbo=0;
// user-id=45947671;
// user-type=mod 
:rokuhodo_!rokuhodo_@rokuhodo_.tmi.twitch.tv PRIVMSG #rokuhodo_ :cheer1000
*/
namespace TwitchLibrary.Models.Messages.IRC.Commands.Native
{
    public class Privmsg : Body
    {
        public int          bits            { get; protected set; }
        public int          mod             { get; protected set; }
        public int          subscriber      { get; protected set; }
        public int          turbo           { get; protected set; }

        public string       display_name    { get; protected set; }
        public string       id              { get; protected set; }
        public string       room_id         { get; protected set; }
        public string       room_name       { get; protected set; }
        public string       user_id         { get; protected set; }
        public string       user_name       { get; protected set; }

        public List<Badge>  badges          { get; protected set; }

        public List<Emote>  emotes          { get; protected set; }

        public UserType     user_type       { get; protected set; }

        public Color        color           { get; protected set; }

        public DateTime     sent_ts         { get; protected set; }
        public DateTime     tmi_sent_ts     { get; protected set; }


        public Privmsg(IrcMessage irc_message) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                bits =          TagConverter.ToGeneric<int>(irc_message.tags, "bits");
                mod =           TagConverter.ToGeneric<int>(irc_message.tags, "mod");
                subscriber =    TagConverter.ToGeneric<int>(irc_message.tags, "subscriber");
                turbo =         TagConverter.ToGeneric<int>(irc_message.tags, "turbo");

                display_name =  TagConverter.ToGeneric<string>(irc_message.tags, "display-name");
                id =            TagConverter.ToGeneric<string>(irc_message.tags, "id");
                room_id =       TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                user_id =       TagConverter.ToGeneric<string>(irc_message.tags, "user-id");

                badges =        TagConverter.ToBadges(irc_message.tags, "badges");

                emotes =        TagConverter.ToEmotes(irc_message.tags, "emotes");

                user_type =     TagConverter.ToUserType(irc_message.tags, "user-type", badges);

                color =         TagConverter.ToColor(irc_message.tags, "color");

                sent_ts =       TagConverter.ToGeneric<DateTime>(irc_message.tags, "sent-ts");
                tmi_sent_ts =   TagConverter.ToGeneric<DateTime>(irc_message.tags, "tmi-sent-ts");
            }

            room_name =     irc_message.middle[0].TextAfter("#");
            user_name =     irc_message.prefix.TextBetween('!', '@');
        }
    }
}
