// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Extensions;

// Example
// :tmi.twitch.tv HOSTTARGET #rokuhodo_ :wyvernslayr 0
namespace TwitchLibrary.Models.Messages.IRC.Commands.Twitch
{
    public class HostTargetStart
    {
        public int      viewers                 { get; protected set; }

        public string   hosting_channel_name    { get; protected set; }
        public string   hosted_channel_name     { get; protected set; }

        public HostTargetStart(IrcMessage irc_message)
        {
            hosting_channel_name =  irc_message.middle[0].TextAfter('#');
            hosted_channel_name =   irc_message.trailing[0];

            viewers = irc_message.trailing[1] == "-" ? 0 : Convert.ToInt32(irc_message.trailing[1]);
        }
    }
}
