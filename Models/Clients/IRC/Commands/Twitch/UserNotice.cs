// standard namespaces
using System;
using System.Collections.Generic;
using System.Drawing;

// project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Clients.IRC.Tags;
using TwitchLibrary.Models.Clients.IRC.Trailing;

// Example
// "
// @badges=subscriber/6,bits/10000;
// color=#004A61;
// display-name=Elazien;
// emotes=;
// id=01ef8e4e-bda1-4476-9204-260f35925e91;
// login=elazien;
// mod=0;
// msg-id=resub;
// msg-param-months=11;
// msg-param-sub-plan-name=Rescue\\sForce;
// msg-param-sub-plan=1000;
// room-id=40972890;
// subscriber=1;
// system-msg=Elazien\\sjust\\ssubscribed\\swith\\sa\\s$4.99\\ssub.\\sElazien\\ssubscribed\\sfor\\s11\\smonths\\sin\\sa\\srow!;
// tmi-sent-ts=1497482317170;
// turbo=0;
// user-id=40297283;
// user-type= 
// :tmi.twitch.tv USERNOTICE #admiralbahroo"

namespace TwitchLibrary.Models.Clients.IRC.Commands.Twitch
{
    public class UserNotice : Body
    {
        public int          mod                     { get; protected set; }
        public int          msg_param_months        { get; protected set; }
        public int          subscriber              { get; protected set; }
        public int          turbo                   { get; protected set; }

        public string       display_name            { get; protected set; }
        public string       id                      { get; protected set; }
        public string       login                   { get; protected set; }
        public string       msg_id                  { get; protected set; }
        public string       msg_param_sub_plan      { get; protected set; }
        public string       msg_param_sub_plan_name { get; protected set; }
        public string       room_id                 { get; protected set; }
        public string       room_name               { get; protected set; }
        public string       system_msg              { get; protected set; }
        public string       user_id                 { get; protected set; }        

        public List<Badge>  badges                  { get; protected set; }

        public UserType     user_type               { get; protected set; }

        public Color        color                   { get; protected set; }

        public DateTime     time_sent_ts            { get; protected set; }

        public UserNotice(IrcMessage irc_message) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                mod =                       TagConverter.ToGeneric<int>(irc_message.tags, "mod");
                msg_param_months =          TagConverter.ToGeneric<int>(irc_message.tags, "msg-param-months");
                subscriber =                TagConverter.ToGeneric<int>(irc_message.tags, "subscriber");
                turbo =                     TagConverter.ToGeneric<int>(irc_message.tags, "turbo");

                display_name =              TagConverter.ToGeneric<string>(irc_message.tags, "display-name");
                id =                        TagConverter.ToGeneric<string>(irc_message.tags, "id");
                login =                     TagConverter.ToGeneric<string>(irc_message.tags, "login");
                msg_id =                    TagConverter.ToGeneric<string>(irc_message.tags, "msg-id");
                msg_param_sub_plan =        TagConverter.ToGeneric<string>(irc_message.tags, "msg-param-sub-plan");
                msg_param_sub_plan_name =   TagConverter.ToGeneric<string>(irc_message.tags, "msg-param-sub-plan-name");
                room_id =                   TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                system_msg =                TagConverter.ToGeneric<string>(irc_message.tags, "system-msg").Replace("\\s", " ");
                user_id =                   TagConverter.ToGeneric<string>(irc_message.tags, "user-id");

                badges =                    TagConverter.ToBadges(irc_message.tags, "badges");

                user_type =                 TagConverter.ToUserType(irc_message.tags, "user-type", badges);

                color =                     TagConverter.ToColor(irc_message.tags, "color");

                time_sent_ts =              TagConverter.ToGeneric<DateTime>(irc_message.tags, "time-sent-ts");
            }

            room_name = irc_message.middle[0].TextAfter("#");
        }
    }
}
