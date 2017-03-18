//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Messages.Whisper;

namespace TwitchLibrary.Events.Clients
{
    public class OnWhisperMessageReceivedEventArgs : EventArgs
    {
        public WhisperMessage whisper_message { get; set; }
    }
}
