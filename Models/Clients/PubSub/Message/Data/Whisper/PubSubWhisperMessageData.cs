using System;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper
{
    public class PubSubWhisperMessageData
    {
        [JsonProperty("message_id")]
        public string message_id { get; internal set; }

        [JsonProperty("id")]
        public string id { get; internal set; }

        [JsonProperty("thread_id")]
        public string thread_id { get; internal set; }

        [JsonProperty("body")]
        public string body { get; internal set; }

        [JsonProperty("sent_ts")]
        public int sent_ts { get; internal set; }

        [JsonProperty("from_id")]
        public string from_id { get; internal set; }

        [JsonProperty("tags")]
        public PubSubWhisperTags tags { get; internal set; }

        [JsonProperty("recipient")]
        public PubSubWhisperRecipient recipient { get; internal set; }

        [JsonProperty("nonce")]
        public string nonce { get; internal set; }
    }
}
