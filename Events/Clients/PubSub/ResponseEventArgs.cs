// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Models.Clients.PubSub.Response;

// imported .dlls
using Newtonsoft.Json;

namespace TwitchLibrary.Events.Clients.PubSub
{
    public class ResponseEventArgs : EventArgs
    {
        public string           raw_message         { get; protected set; }

        public PubSubResponse   response_message    { get; protected set; }

        public ResponseEventArgs(string _raw_message)
        {
            raw_message         = _raw_message;

            response_message    = JsonConvert.DeserializeObject<PubSubResponse>(_raw_message);
        }
    }
}
