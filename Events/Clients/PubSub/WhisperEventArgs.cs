// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper;

// imported .dlls
using Newtonsoft.Json;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class WhisperEventArgs : EventArgs
    {
        public string           raw_message             { get; protected set; }

        public PubSubMessage    pub_sub_message         { get; protected set; }
        public WhisperMessage   whisper_message         { get; protected set; }
        public WhisperData      whisper_message_data    { get; protected set; }

        public WhisperEventArgs(string _raw_message, PubSubMessage _pub_sub_message)
        {
            raw_message             = _raw_message;

            pub_sub_message         = _pub_sub_message;
            whisper_message         = JsonConvert.DeserializeObject<WhisperMessage>(pub_sub_message.data.message);
            whisper_message_data    = JsonConvert.DeserializeObject<WhisperData>(whisper_message.data);
        }
    }
}
