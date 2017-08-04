// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class MessageEventArgs : EventArgs
    {
        public string           raw_message     { get; protected set; }

        public PubSubMessage    pub_sub_message { get; protected set; }

        public MessageEventArgs(string _raw_message, PubSubMessage _pub_sub_message)
        {
            raw_message     = _raw_message;

            pub_sub_message = _pub_sub_message;
        }
    }
}
