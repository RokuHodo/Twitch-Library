// standard namespaces
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;

// Example
// @room-id=45947671;slow=10 :tmi.twitch.tv ROOMSTATE #rokuhodo_
namespace TwitchLibrary.Models.Clients.IRC.Commands.Twitch
{
    public class RoomStateChange
    {
        public string                       room_name       { get; protected set; }
        public string                       room_id         { get; protected set; }

        public KeyValuePair<string, string> changed_state   { get; protected set; }

        public RoomStateChange(IrcMessage irc_message)
        {
            if (irc_message.contains_tags)
            {
                room_id = TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                foreach(KeyValuePair<string, string> pair in irc_message.tags)
                {
                    if(pair.Key != "room-id")
                    {
                        changed_state = pair;

                        break;
                    }
                }
            }

            room_name = irc_message.middle[0].TextAfter('#');
        }
    }
}