//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.Tags;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages
{
    public class Message
    {
        public string body { get; private set; }

        public MessageEmotes emotes { get; private set; }

        public Message(IrcMessage irc_message)
        {            
            if (irc_message.contains_tags)
            {
                emotes = TagConverter.ToEmotes(irc_message.tags, "emotes");
            }

            body = GetBody(irc_message.trailing);
        }

        /// <summary>
        /// Gets the body of the message that was typed in chat by the user.
        /// </summary>
        private string GetBody(string[] trailing)
        {
            string body = string.Empty;

            if (!trailing.isValidArray())
            {
                return body;
            }

            foreach (string element in trailing)
            {
                body += " " + element;
            }

            body = body.RemovePadding();            

            return body;
        }   
    }
}
