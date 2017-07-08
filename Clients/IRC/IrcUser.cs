using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchLibrary.Clients.IRC
{
    public class IrcUser
    {
        public string nick;
        public string pass;

        public IrcUser(string _nick, string _pass)
        {
            nick = _nick;
            pass = _pass;
        }

        public IrcUser()
        {

        }
    }
}
