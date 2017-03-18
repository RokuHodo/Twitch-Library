//project namespaces
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Whisper
{
    public class WhisperSender : Sender
    {
        public WhisperSender(IrcMessage irc_message) : base(irc_message)
        {

        }
    }    
}