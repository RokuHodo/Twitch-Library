//standard namespaces
using System;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper
{
    public class PubSubWhisperMessage
    {
        [JsonProperty("type")]
        public string type { get; internal set; }

        [JsonProperty("data")]
        public string data { get; internal set; }

        [JsonProperty("data_object")]
        public PubSubWhisperMessageData data_object { get; internal set; }
    }
}
