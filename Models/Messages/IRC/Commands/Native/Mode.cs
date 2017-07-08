// project namespaces
using TwitchLibrary.Extensions;

// example
// :jtv MODE #rokuhodo_ +o rokuhodo_
namespace TwitchLibrary.Models.Messages.IRC.Commands.Native
{
    public class Mode
    {
        public bool     modded          { get; protected set; }

        public string   mode            { get; protected set; }
        public string   user_name       { get; protected set; }
        public string   channel_name    { get; protected set; }

        public Mode(IrcMessage irc_message)
        {
            mode =          irc_message.middle[1];

            user_name =     irc_message.middle[2];
            channel_name =  irc_message.middle[0].TextAfter("#");
        }
    }
}
