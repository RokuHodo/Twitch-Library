﻿// standard namespaces
using System.Collections.Generic;
using System.Drawing;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper
{
    public class PubSubWhisperRecipient
    {
        [JsonProperty("id")]
        public string id { get; internal set; }

        [JsonProperty("username")]
        public string username { get; internal set; }

        [JsonProperty("display_name")]
        public string display_name { get; internal set; }

        [JsonProperty("color")]
        public Color color { get; internal set; }

        [JsonProperty("user_type")]
        public string user_type { get; internal set; }

        [JsonProperty("turbo")]
        public bool turbo { get; internal set; }

        [JsonProperty("badges")]
        public List<PubSubWhisperBadges> badges { get; internal set; }

        [JsonProperty("profile_image")]
        public string profile_image { get; internal set; }
    }
}
