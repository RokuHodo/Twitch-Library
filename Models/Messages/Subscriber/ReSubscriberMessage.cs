//standard namespaces
using System;

//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Messages;
using TwitchLibrary.Models.Messages.IRC;

namespace TwitchLibrary.Models.Messages.Subscriber
{
    public class ResubscriberMessage : Message
    {
        //twitch prime        
        public bool is_premium { get; protected set; }

        public int msg_param_months { get; protected set; }

        public string msg_id { get; protected set; }
        public string room_id { get; protected set; }
        public string room_name { get; protected set; }
        public string system_msg { get; protected set; }
        public string login { get; protected set; }

        public ReSubcriberSender sender { get; protected set; }

        public ResubscriberMessage(IrcMessage irc_message, string oauth_token) : base(irc_message)
        {
            if (irc_message.contains_tags)
            {
                msg_param_months = TagConverter.ToGeneric<int>(irc_message.tags, "msg-param-months");

                msg_id = TagConverter.ToGeneric<string>(irc_message.tags, "msg-id");
                room_id = TagConverter.ToGeneric<string>(irc_message.tags, "room-id");
                room_name = GetRoomName(irc_message, oauth_token);
                system_msg = TagConverter.ToGeneric<string>(irc_message.tags, "system-msg").Replace("\\s", " ");
                login = TagConverter.ToGeneric<string>(irc_message.tags, "login");

                is_premium = system_msg.Contains("Twitch Prime");
            }            

            sender = new ReSubcriberSender(irc_message, login);
        }

        private string GetRoomName(IrcMessage irc_message, string oauth_token)
        {
            string room = string.Empty;

            try
            {
                //this should always work unless Twitch changes IRC spec, add a try just to be safe
                room = irc_message.middle[0].TextAfter("#");
            }
            catch (Exception excepion)
            {
                LibraryDebug.Error(LibraryDebugMethod.GET, nameof(room), LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLineFormatted(nameof(excepion), excepion.Message);

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
