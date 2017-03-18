//project namespaces
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Subscriber
{
    public class ReSubcriberSender : Sender
    {
        public bool mod { get; protected set; }
        public bool subscriber { get; protected set; }

        public ReSubcriberSender(IrcMessage irc_message, string login) : base(irc_message)
        {
            mod = TagConverter.ToBool(irc_message.tags, "mod");
            subscriber = TagConverter.ToBool(irc_message.tags, "subscriber");

            name = login;
        }
    }
}
