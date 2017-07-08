using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
