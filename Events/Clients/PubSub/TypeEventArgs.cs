// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Enums.Clients.PubSub;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class TypeEventArgs : EventArgs
    {
        public string       raw_message { get; protected set; }

        public PubSubType   type        { get; protected set; }

        public TypeEventArgs(string _raw_message, PubSubType _type)
        {
            raw_message = _raw_message;

            type        = _type;
        }
    }
}
