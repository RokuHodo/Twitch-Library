//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class BitsReceivedEventArgs : EventArgs
    {
        public string data { get; internal set; }
        public PubSubBitsMessage bits_message { get; internal set; }
    }
}
