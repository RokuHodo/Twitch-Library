// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class HostTargetStartEventArgs : IrcMessageEventArgs
    {
        public HostTargetStart host_target_start_message { get; protected set; }

        public HostTargetStartEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            host_target_start_message = new HostTargetStart(_irc_message);
        }
    }
}
