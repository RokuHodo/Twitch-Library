// project namespaces
using TwitchLibrary.Extensions;

// Example
// :rokubotto!rokubotto@rokubotto.tmi.twitch.tv JOIN #rokuhodo_
namespace TwitchLibrary.Models.Messages.IRC.Commands.Native
{
    public class Join
    {
        public string user_name     { get; protected set; }
        public string channel_name  { get; protected set; }

        public Join(IrcMessage irc_message)
        {
            user_name =     irc_message.prefix.TextBetween('!', '@');
            channel_name =  irc_message.middle[0].TextAfter('#');
        }
    }
}
