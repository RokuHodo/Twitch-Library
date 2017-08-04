// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Subscriptions;

// imported .dlls
using Newtonsoft.Json;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class SubscriberEventArgs : EventArgs
    {
        public string               raw_message             { get; protected set; }

        public PubSubMessage        pub_sub_message         { get; protected set; }
        public SubscriptionsMessage subscription_message    { get; protected set; }

        public SubscriberEventArgs(string _raw_message, PubSubMessage _pub_sub_message)
        {
            raw_message             = _raw_message;

            pub_sub_message         = _pub_sub_message;
            subscription_message    = JsonConvert.DeserializeObject<SubscriptionsMessage>(pub_sub_message.data.message);
        }
    }
}
