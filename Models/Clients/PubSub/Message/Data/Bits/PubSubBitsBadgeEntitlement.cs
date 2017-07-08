﻿// standard namespaces
using System;

// imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits
{
    public class PubSubBitsBadgeEntitlement
    {
        [JsonProperty("new_version")]
        public int new_version { get; internal set; }

        [JsonProperty("previous_version")]
        public int previous_version { get; internal set; }
    }
}
