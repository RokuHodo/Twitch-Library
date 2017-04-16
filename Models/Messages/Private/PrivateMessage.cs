//standard namespaces
using System;

//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Private
{
    public class PrivateMessage : Message
    {
        public int bits { get; protected set; }                 

        public long sent_ts { get; protected set; }              
        public long tmi_sent_ts { get; protected set; }          

        public string id { get; protected set; }
        public string room_id { get; protected set; }
        public string room_name { get; protected set; }

        public PrivateSender sender { get; protected set; }                  

        public PrivateMessage(IrcMessage irc_message, string oauth_token) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                bits = TagConverter.ToGeneric<int>(irc_message.tags, "bits");

                sent_ts = TagConverter.ToGeneric<long>(irc_message.tags, "sent-ts");
                tmi_sent_ts = TagConverter.ToGeneric<long>(irc_message.tags, "tmi-sent-ts");

                id = TagConverter.ToGeneric<string>(irc_message.tags, "id");
                room_id = TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                room_name = GetRoomName(irc_message, oauth_token);
            }

            sender = new PrivateSender(irc_message);
        }          
        
        private string GetRoomName(IrcMessage irc_message, string oauth_token)
        {
            string room = string.Empty;

            try
            {
                //this should always work unless Twitch changes IRC spec, add a try just to be safe
                room = irc_message.middle[0].TextAfter("#");
            }
            catch(Exception excepion)
            {
                LibraryDebug.Error(LibraryDebugMethod.GET, nameof(room), LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(excepion), excepion.Message);

                if (irc_message.contains_tags)
                {
                    LibraryDebug.Warning("Getting " + nameof(room) + " from the Twitch API");

                    //slower but is always guaranteed to work
                    room = new TwitchApi("", oauth_token).GetUser(room_id).name;
                }                
            }

            return room;
        }
    }
}
