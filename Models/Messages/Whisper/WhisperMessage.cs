//standard namespaces
using System;

//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Whisper
{
    public class WhisperMessage : Message
    {
        public string message_id { get; protected set; }
        public string thread_id { get; protected set; }
        public string recipient_id { get; protected set; }
        public string recipient_name { get; protected set; }

        public WhisperSender sender { get; protected set; }                  

        public WhisperMessage(IrcMessage irc_message, string oauth_token) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                message_id = TagConverter.ToGeneric<string>(irc_message.tags, "message-id");
                thread_id = TagConverter.ToGeneric<string>(irc_message.tags, "thread-id");
                recipient_id = thread_id.TextAfter("_");
                recipient_name = GetRecipientName(irc_message, oauth_token);
            }

            sender = new WhisperSender(irc_message);
        }      

        private string GetRecipientName(IrcMessage irc_message, string oauth_token)
        {
            string recipient = string.Empty;

            try
            {
                //this should always work unless Twitch changes IRC spec, add a try just to be safe
                recipient = irc_message.middle[0];
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.GET, nameof(recipient_name), LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);

                if (irc_message.contains_tags)
                {
                    LibraryDebug.Warning("Getting " + nameof(recipient) + " from the Twitch API");

                    //slower but is always guaranteed to work
                    recipient = new TwitchApi("", oauth_token).GetUser(recipient_id).name;
                }                
            }

            return recipient;
        }
    }
}
