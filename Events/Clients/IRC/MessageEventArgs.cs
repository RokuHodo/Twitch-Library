using System;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class MessageEventArgs : EventArgs
    {
        public string data { get; internal set; }

        public MessageEventArgs(string _data)
        {
            data = _data;
        }
    }
}
