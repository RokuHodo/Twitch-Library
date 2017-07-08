// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class WhisperReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubWhisperMessage whisper_message { get; internal set; }
        public PubSubWhisperMessageData whisper_message_data { get; internal set; }
    }
}
