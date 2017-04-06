//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Whisper;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class WhisperMessageReceivedEventArgs : EventArgs
    {
        public WhisperMessage whisper_message { get; set; }
    }
}
