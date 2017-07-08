// standard namepaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Extensions;

// Example
// :rokubotto.tmi.twitch.tv 366 rokubotto #rokuhodo_ :End of /NAMES list

namespace TwitchLibrary.Models.Messages.IRC.Commands.Native
{
    public class Names
    {
        public string       user_name       { get; protected set; }
        public string       channel_name    { get; protected set; }

        public List<string> names           { get; protected set; }

        public Names(IrcMessage irc_message, List<string> _names)
        {
            user_name =     irc_message.middle[0];
            channel_name =  irc_message.middle[1].TextAfter('#');

            names =         _names;
        }
    }
}
