using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitchLibrary.Extensions;
using TwitchLibrary.Models.Messages.Private;

namespace TwitchLibrary.Models.Messages.Subscriber
{
    public class NewSubcriberSender
    {
        public string name { get; protected set; }
        public string display_name { get; protected set; }

        public NewSubcriberSender(PrivateMessage private_message)
        {
            display_name = private_message.body.TextBefore(" ");
            name = display_name.ToLower();
        }
    }
}
