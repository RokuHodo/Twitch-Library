// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Commerce;

// imported .dlls
using Newtonsoft.Json;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class CommerceEventArgs : EventArgs
    {
        public string           raw_message         { get; protected set; }

        public PubSubMessage    pub_sub_message     { get; protected set; }
        public CommerceMessage  commerce_message    { get; protected set; }

        public CommerceEventArgs(string _raw_message, PubSubMessage _pub_sub_message)
        {
            raw_message         = _raw_message;

            pub_sub_message     = _pub_sub_message;
            commerce_message    = JsonConvert.DeserializeObject<CommerceMessage>(pub_sub_message.data.message);
        }
    }
}
