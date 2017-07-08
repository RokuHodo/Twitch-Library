// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Native;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Native
{
    public class PartEventArgs : IrcMessageEventArgs
    {
        public Part part_message { get; protected set; }

        public PartEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            part_message = new Part(_irc_message);
        }
    }
}
