// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Extensions;

// Example
// :tmi.twitch.tv HOSTTARGET #rokuhodo_ :- 0
namespace TwitchLibrary.Models.Messages.IRC.Commands.Twitch
{
    public class HostTargetEnd
    {
        public int      viewers                 { get; protected set; }

        public string   hosting_channel_name    { get; protected set; }

        public HostTargetEnd(IrcMessage irc_message)
        {
            hosting_channel_name =  irc_message.middle[0].TextAfter('#');
            viewers =               Convert.ToInt32(irc_message.trailing[1]);
        }
    }
}