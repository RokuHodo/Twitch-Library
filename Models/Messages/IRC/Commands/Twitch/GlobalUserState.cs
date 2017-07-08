// standard namespaces
using System.Drawing;
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Helpers.Messages;

namespace TwitchLibrary.Models.Messages.IRC.Commands.Twitch
{
    public class GlobalUserState
    {
        public int          turbo           { get; protected set; }
                
        public string       display_name    { get; protected set; }
        public string       user_id         { get; protected set; }

        public List<string> emote_sets      { get; protected set; }

        public UserType     user_type       { get; protected set; }

        public Color        color           { get; protected set; }
        
        public GlobalUserState(IrcMessage irc_message)
        {
            if (irc_message.contains_tags)
            {
                turbo =         TagConverter.ToGeneric<int>(irc_message.tags, "turbo");

                display_name =  TagConverter.ToGeneric<string>(irc_message.tags, "display-name");
                user_id =       TagConverter.ToGeneric<string>(irc_message.tags, "user-id");

                emote_sets =    TagConverter.ToList<string>(irc_message.tags, "emote-sets");

                user_type =     TagConverter.ToEnum<UserType>(irc_message.tags, "user-type");

                color =         TagConverter.ToColor(irc_message.tags, "color");
            }
        }
    }
}
