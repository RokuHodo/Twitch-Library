using System;

using TwitchLibrary.Models.Messages.Whisper;

namespace TwitchLibrary.Events.Clients
{
    public class OnWhisperMessageReceivedEventArgs : EventArgs
    {
        public WhisperMessage whisper_message { get; set; }
    }
}
