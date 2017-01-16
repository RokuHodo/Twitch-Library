using System;

using TwitchLibrary.Models.Messages.Whisper;

namespace TwitchLibrary.Events.Clients
{
    public class OnWhisperMessageReceivedArgs : EventArgs
    {
        public WhisperMessage whisper_message { get; set; }
    }
}
