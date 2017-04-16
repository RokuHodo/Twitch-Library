//standard namespaces
using System;
using System.Collections.Generic;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class NamesReceivedEventArgs : EventArgs
    {
        public List<string> names { get; internal set; }
    }
}
