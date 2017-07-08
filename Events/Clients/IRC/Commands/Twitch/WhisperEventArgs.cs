// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class WhisperEventArgs : IrcMessageEventArgs
    {
        public Whisper whisper_message { get; protected set; }

        public WhisperEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            whisper_message = new Whisper(_irc_message);
        }
    }
}
