// project namespaces
using TwitchLibrary.Extensions;

namespace TwitchLibrary.Models.Clients.IRC.Trailing
{
    public class Body
    {
        public string body { get; protected set; }

        internal Body(IrcMessage irc_message)
        {
            body = GetBody(irc_message);
        }

        private string GetBody(IrcMessage irc_message)
        {
            string _body = string.Empty;

            if (!irc_message.trailing.isValid())
            {
                return _body;
            }

            _body = string.Join(" ", irc_message.trailing).RemovePadding();

            return _body;
        }
    }
}
