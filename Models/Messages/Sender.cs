using System;
using System.Drawing;

//project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages
{
    public class Sender
    {      
        public bool turbo { get; protected set; }

        public string name { get; protected set; }
        public string display_name { get; protected set; }
        public string user_id { get; protected set; }

        public string[] badges { get; protected set; }

        public UserType user_type { get; protected set; }    

        public Color color { get; protected set; }        

        public Sender(IrcMessage irc_message)
        {
            name = irc_message.prefix.TextBefore("!");

            if (irc_message.contains_tags)
            {
                turbo = TagConverter.ToBool(irc_message.tags, "turbo");

                display_name = TagConverter.ToGeneric<string>(irc_message.tags, "display-name");
                user_id = TagConverter.ToGeneric<string>(irc_message.tags, "user-id");

                badges = TagConverter.ToBadges(irc_message.tags, "badges");

                user_type = Array.Exists(badges, badge => badge.Contains("broadcaster")) ? UserType.broadcaster : TagConverter.ToEnum<UserType>(irc_message.tags, "user-type");

                color = TagConverter.ToColor(irc_message.tags, "color");
            }            
        }
    }    
}