// project namespaces
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.IRC.Commands.Twitch;

namespace TwitchLibrary.Events.Clients.IRC.Commands.Twitch
{
    public class HostTargetEndEventArgs : IrcMessageEventArgs
    {
        public HostTargetEnd host_target_end_message { get; protected set; }

        public HostTargetEndEventArgs(string _raw_message, IrcMessage _irc_message) : base(_raw_message, _irc_message)
        {
            host_target_end_message = new HostTargetEnd(_irc_message);
        }
    }
}
