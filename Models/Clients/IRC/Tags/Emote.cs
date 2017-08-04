// standard namespaces
using System.Collections.Generic;

namespace TwitchLibrary.Models.Clients.IRC.Tags
{
    public class Emote
    {
        public string           id      { get; internal set; }

        public List<EmoteRange> ranges  { get; internal set; }  
    }
}
