//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Models.API.Channels;

namespace TwitchLibrary.Events.Clients.IRC
{
    public class NewFollowerEventArgs : EventArgs
    {
        public Follower follower { get; set; }
    }
}
