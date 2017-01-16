//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Private
{
    public class PrivateSender : Sender
    {
        public bool mod { get; protected set; }                   
        public bool subscriber { get; protected set; }               

        public PrivateSender(IrcMessage irc_message) : base(irc_message)
        {
            name = irc_message.prefix.TextBetween(':', '!');

            if (irc_message.contains_tags)
            {
                mod = TagConverter.ToBool(irc_message.tags, "mod");
                subscriber = TagConverter.ToBool(irc_message.tags, "subscriber");
            }            
        }
    }    
}