using System;

using TwitchLibrary.Models.API.Channels;

namespace TwitchLibrary.Events.Clients
{
    public class OnNewFollowerEventArgs : EventArgs
    {
        public Follower follower { get; set; }
    }
}
