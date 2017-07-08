// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Subscriptions;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class SubscriptionReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubSubscriptionsMessage subscription_message { get; internal set; }
    }
}
