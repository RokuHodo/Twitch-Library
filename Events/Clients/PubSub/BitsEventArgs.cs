// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits;

// imported .dlls
using Newtonsoft.Json;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class BitsEventArgs : EventArgs
    {
        public string           raw_message     { get; protected set; }

        public PubSubMessage    pub_sub_message { get; protected set; }
        public BitsMessage      bits_message    { get; protected set; }

        public BitsEventArgs(string _raw_message, PubSubMessage _pub_sub_message)
        {
            raw_message = _raw_message;

            pub_sub_message = _pub_sub_message;
            bits_message    = JsonConvert.DeserializeObject<BitsMessage>(pub_sub_message.data.message);
        }
    }
}
