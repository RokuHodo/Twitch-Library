// standard namespaces
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;

// project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Clients.IRC;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Events.Clients.IRC;
using TwitchLibrary.Models.Clients.IRC.Templates;
using TwitchLibrary.Events.Clients.IRC.Commands.Twitch;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.API.Users;

namespace TwitchLibrary.Clients.IRC
{
    public class TwitchIrcClient : IrcClient
    {
        #region Fields

        // private

        private static string                               twitch_host = "irc.chat.twitch.tv";

        private Queue<MessageTemplate>                      privmsg_queue;
        private Queue<MessageTemplate>                      whisper_queue;

        private Timer                                       privmsg_queue_timer;
        private Timer                                       whisper_queue_timer;        

        // public               

        /// <summary>
        /// If set to true, the client will automatically reconnect when a 'RECONNECT' message is received from Twitch.
        /// </summary>
        public bool                                         auto_reconnect;

        /// <summary>
        /// If set to true, 'tags' are automatically requested when a successful connected is established.
        /// </summary>
        public bool                                         request_tags;

        /// <summary>
        /// If set to true, 'commands' are automatically requested when a successful connected is established.
        /// </summary>
        public bool                                         request_commands;

        /// <summary>
        /// If set to true, 'membership' is automatically requested when a successful connected is established.
        /// </summary>
        public bool                                         request_membership;

        // TODO: (IrcClient) Implement follower service and OnNewFollower
        // public event EventHandler<OnNewFollowerEventArgs>   OnUserFollowed;                                        

        /// <summary>
        /// Raised when the client received a 'RECONNECT' from the Twitch IRC.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs>      OnReconnect;

        /// <summary>
        /// Raised when a whisper message has been received in a channel that the client has joined.
        /// </summary>
        public event EventHandler<WhisperEventArgs>         OnWhisper;        

        /// <summary>
        /// Raised when a user gets timed out in a channel or when the chat room histroy gets cleared. Requires 'commands' to be requested.
        /// </summary>
        public event EventHandler<ClearChatEventArgs>       OnClearChat;

        /// <summary>
        /// Raised when a user successfully logs in. Requires 'commands' to be requested.
        /// </summary>
        public event EventHandler<GlobalUserStateEventArgs> OnGlobalUserState;

        /// <summary>
        /// Raised when a user subscribes to a channel that the client has joined. This event includes subs, resubs, and charity. Requires 'commands' to be requested.
        /// </summary>
        public event EventHandler<UserNoticeEventArgs>      OnUserNotice;

        /// <summary>
        /// Raised when a user joins a channel or the state of a room gets changed.
        /// </summary>
        public event EventHandler<RoomStateEventArgs>       OnRoomState;

        /// <summary>
        /// Raised when a user the state of a room is changed. Only the changes state tag is returned.
        /// </summary>
        public event EventHandler<RoomStateChangeEventArgs> OnRoomStateChange;

        /// <summary>
        /// Raised when a user joins a room or sends a PRIVMSG to a channel.
        /// </summary>
        public event EventHandler<UserStateEventArgs>       OnUserState;

        /// <summary>
        /// Raised when a general message is sent from the server about a channel.
        /// </summary>
        public event EventHandler<NoticeEventArgs>          OnNotice;

        /// <summary>
        /// Raised when a channel stops hosting another channel.
        /// </summary>
        public event EventHandler<HostTargetEndEventArgs>   OnHostTargetEnd;

        /// <summary>
        /// Raised when a channel starts hosting another channel.
        /// </summary>
        public event EventHandler<HostTargetStartEventArgs> OnHostTargetStart;

        #endregion

        #region Properties

        // public

        /// <summary>
        /// The user id of the Twitch account associasted with the irc client.
        /// </summary>
        public string                                       user_id         { get; private set; }

        /// <summary>
        /// The name of the Twitch account associasted with the irc client.
        /// </summary>
        public string                                       user_name       { get; private set; }

        /// <summary>
        /// The formatted display name of the Twitch account associasted with the irc client.
        /// </summary>
        public string                                       display_name    { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a <see cref="TwitchIrcClient"/> <see cref="object"/>, a derived class from <see cref="IrcClient"/>.
        /// </summary>
        /// <param name="irc_user">The user that will be logged into the IRC.</param>
        /// <param name="ssl">Determines whether to use the SSL port when logging in.</param>
        /// <exception cref="ArgumentException">Thrown when the irc_user fields are improperly formatted.</exception>
        public TwitchIrcClient(IrcUser irc_user, bool ssl = false) : base(twitch_host, ssl == false ? 6667 : 443, irc_user)
        {
            // allow upper case to not be too strict and just ToLower() it later
            Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]{3,24}$");
            if (!regex.IsMatch(irc_user.nick))
            {
                throw new ArgumentException(Error.EXCEPTION_ARGUMENT_TWITCH_NAME, nameof(irc_user.nick));
            }
            /*
            // TODO: (TwitchIrcClient) - require that all tokens be prefixed with "oauth:" rather than adding it ourselves
            if (!irc_user.pass.StartsWith("oauth:"))
            {
                throw new ArgumentException(Error.EXCEPTION_ARGUMENT_TWITCH_OAUTH, nameof(irc_user.pass));
            }
            */
            auto_reconnect = true;

            request_tags                = true;
            request_commands            = true;
            request_membership          = true;

            OnConnected                 += new EventHandler<EventArgs>(Callback_OnConnected);
            OnDisconnected              += new EventHandler<EventArgs>(Callback_OnDisconnected);
            OnIrcMessage                += new EventHandler<IrcMessageEventArgs>(Callback_OnIrcMessage);

            User user                   = new TwitchApiOAuth(irc_user.pass).GetUser();
            user_id                     = user._id;
            user_name                   = user.name;
            display_name                = user.display_name;

            privmsg_queue               = new Queue<MessageTemplate>();
            whisper_queue               = new Queue<MessageTemplate>();

            privmsg_queue_timer         = new Timer(1500);
            privmsg_queue_timer.Elapsed += ProcressEnqueuedChatCommands;

            whisper_queue_timer         = new Timer(1500);
            whisper_queue_timer.Elapsed += ProcressEnqueuedWhispers;
        }               

        #endregion      

        #region Event handling

        /// <summary>
        /// Fired when the client successfuly connects to Twitch IRC.
        /// Handles auto requesting tags, commands, and membership and handles reconnecting.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void Callback_OnConnected(object sender, EventArgs e)
        {
            privmsg_queue_timer.Enabled = true;
            whisper_queue_timer.Enabled = true;
            
            if (request_tags)
            {
                RequestTags();
            }

            if (request_commands)
            {
                RequestCommands();
            }

            if (request_membership)
            {
                RequestMembership();
            }
        }

        /// <summary>
        /// Fired when the client successfuly disconnects from the Twitch IRC.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void Callback_OnDisconnected(object sender, EventArgs e)
        {
            privmsg_queue.Clear();
            whisper_queue.Clear();

            privmsg_queue_timer.Enabled = false;
            whisper_queue_timer.Enabled = false;
        }

        /// <summary>
        /// Fired when a message is receieved from the Twitch IRC.
        /// The message is parsed and processed according to what irc command it contains.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void Callback_OnIrcMessage(object sender, IrcMessageEventArgs e)
        {
            switch (e.irc_message.command)
            {
                #region Requires no requests to be sent
                
                case "WHISPER":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - WHISPER tags only sent when /tags is requested
                        OnWhisper.Raise(this, new WhisperEventArgs(e.raw_message, e.irc_message));
                    }
                    break;

                #endregion

                #region Requires 'commands' to be requested

                case "USERNOTICE":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - USERNOTICE requires /commands, tags only sent when /tags is requested
                        OnUserNotice.Raise(this, new UserNoticeEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "CLEARCHAT":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - CLEARCHAT requires /commands, tags only sent when /tags is requested
                        OnClearChat.Raise(this, new ClearChatEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "GLOBALUSERSTATE":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - GLOBALUSERSTATE requires /commands, tags only sent when /tags is requested
                        // TODO: (TwitchIrcClient) IRC Command - GLOBALUSERSTATE check to make sure the model matches what is being received through the IRC
                        OnGlobalUserState.Raise(this, new GlobalUserStateEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "ROOMSTATE":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - ROOMSTATE requires /commands, tags only sent when /tags is requested
                        // NOTE: (TwitchIrcClient) IRC Command - ROOMSTATE this will break if Twitch ever includes more than just the room-id and the changed state tags
                        if (e.irc_message.contains_tags && e.irc_message.tags.Count == 2)
                        {
                            OnRoomStateChange.Raise(this, new RoomStateChangeEventArgs(e.raw_message , e.irc_message));
                        }
                        else
                        {
                            OnRoomState.Raise(this, new RoomStateEventArgs(e.raw_message , e.irc_message));
                        }
                    }
                    break;
                case "USERSTATE":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - USERSTATE requires /commands, tags only sent when /tags is requested
                        OnUserState.Raise(this, new UserStateEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "RECONNECT":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - RECONNECT requires /commands
                        if (auto_reconnect)
                        {
                            ReconnectAsync();
                        }

                        OnReconnect.Raise(this, new IrcMessageEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "NOTICE":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - NOTICE requires /commands
                        OnNotice.Raise(this, new NoticeEventArgs(e.raw_message, e.irc_message));
                    }
                    break;
                case "HOSTTARGET":
                    {
                        // NOTE: (TwitchIrcClient) IRC Command - HOSTTARGET requires /commands
                        if (e.irc_message.trailing[0] == "-")
                        {
                            OnHostTargetEnd.Raise(this, new HostTargetEndEventArgs(e.raw_message , e.irc_message));
                        }
                        else
                        {
                            OnHostTargetStart.Raise(this, new HostTargetStartEventArgs(e.raw_message, e.irc_message));
                        }
                    }
                    break;

                #endregion
            }
        }
        
        #endregion               

        #region Twitch requests

        public void RequestTags()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Requesting 'tags' from Twitch IRC.");
            Send("CAP REQ :twitch.tv/tags");
        }

        public void RequestCommands()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Requesting 'commands' from Twitch IRC.");
            Send("CAP REQ :twitch.tv/commands");
        }

        public void RequestMembership()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Requesting 'membership' from Twitch IRC.");
            Send("CAP REQ :twitch.tv/membership");
        }

        #endregion

        #region Twitch chat commands

        // TODO: (TwitchIrcClient) Twitch commands - Re-test these to make sure I didn't break these

        // TODO: (TwitchIrcClient) Chat Command - IgnoreUser - Not possible directly through IRC?
        // TODO: (TwitchIrcClient) Chat Command - UnignoreUser - Not possible directly through IRC? 

        #region Basic commands

        /// <summary>
        /// Display a list of all chat moderators for that channel the client is associated with.
        /// The list will not pop in in any chat room, but a NOTICE will be received with the list of mods.
        /// </summary>
        public void Mods()
        {
            Mods(user_name);
        }

        /// <summary>
        /// Display a list of all chat moderators for that specific channel.
        /// The list will not pop in in any chat room, but a NOTICE will be received with the list of mods.
        /// </summary>
        /// <param name="room_name">The room to check for mods in.</param>
        public void Mods(string room_name)
        {
            EnqueuePrivmsg(room_name, ".mods");
        }

        /// <summary>
        /// Changes the display name color of the client in Twitch chat.
        /// Can be used by any client, regardless if the associated user has Turbo/Twitch Prime or not.
        /// Chat windows need to be refreshed for the changes to take effect.
        /// </summary>
        /// <param name="color">The color to change the display name too.</param>
        public void Color(DisplayColor color)
        {
            string display_color = color.ToString().ToLower();
            Color(display_color);
        }

        /// <summary>
        /// Changes the display name color of the client in Twitch chat.
        /// This can only be used by clients associated with a user that has turbo or Twitch Prime.
        /// If a user associated with the client does not have Tubro or Twitch Prime, the action will not succeed.
        /// Chat windows need to be refreshed for the changes to take effect.
        /// </summary>
        /// <param name="color">The color to change the display name too.</param>
        public void Color(Color color)
        {
            string html_color = ColorTranslator.ToHtml(color);
            Color(html_color);
        }

        /// <summary>
        /// Changes the display name color of the client in Twitch chat.
        /// The color can be either a string version of <see cref="DisplayColor"/> for any client or an html hex color value for clients with Turbo or Twitch Prime.
        /// If a client that is not associated with a user with Turbo or Twitch Prime attempts to use a html hex color, the action will fail.
        /// Chat windows need to be refreshed for the changes to take effect.
        /// </summary>
        /// <param name="color">
        /// The color to change the display name too.
        /// This can be either a string version of <see cref="DisplayColor"/> or an html hex color value.
        /// </param>
        public void Color(string color)
        {
            EnqueuePrivmsg(user_name.ToLower(), ".color " + color);
        }

        /// <summary>
        /// Sends a message with the color based on your chat name color,
        /// </summary>
        /// <param name="room_name">The room to send the channel too.</param>
        /// <param name="message">The message to be sent to the room.</param>
        public void Me(string room_name, string message)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".me " + message);
        }

        /// <summary>
        /// Disconnects the cliuent from the chat server but remains connected to the IRC.
        /// </summary>
        public void DisconnectChat()
        {
            EnqueuePrivmsg(user_name.ToLower(), ".disconnect");
        }

        #endregion

        #region Moderator commands

        /// <summary>
        /// Purges a user in a specific room for 1 second with an optional reason.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to purge the user in.</param>
        /// <param name="user_name">The user to purge.</param>
        /// <param name="reason">Reason for the purge.</param>
        public void Purge(string room_name, string user_name, string reason = "")
        {
            Timeout(room_name, user_name, 1, reason);
        }

        /// <summary>
        /// Times out a user in a specific room for a specified amount of time with an optional reason.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to timout the user in.</param>
        /// <param name="user_name">The user to time out.</param>
        /// <param name="seconds">The length of the time outs in seconds. Default is 600 seconds (10 minutes), minimum is 1 second.</param>
        /// <param name="reason">Reason for the time out.</param>
        public void Timeout(string room_name, string user_name, int seconds = 600, string reason = "")
        {
            EnqueuePrivmsg(room_name.ToLower(), ".timeout " + user_name.ToLower() + " " + seconds.ClampMin(1, 1) + " " + reason);
        }

        /// <summary>
        /// Bans a user in a specific room with an optional reason
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to ban the user in.</param>
        /// <param name="user_name">The user to ban.</param>
        /// <param name="reason">Reason for the ban.</param>
        public void Ban(string room_name, string user_name, string reason = "")
        {
            EnqueuePrivmsg(room_name.ToLower(), ".ban " + user_name.ToLower() + " " + reason);
        }

        /// <summary>
        /// Unbans a user in a specific room.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to unban the user in.</param>
        /// <param name="user_name">The user to unban.</param>
        public void Unban(string room_name, string user_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".unban " + user_name.ToLower());
        }

        /// <summary>
        /// Puts a room into slow mode.
        /// Restricts users to only be able to send one message every so often.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put into slow mode.</param>
        /// <param name="seconds">How frequently users are allowed to send messages. Default is 1 second, minumum is 1 second.</param>
        public void Slow(string room_name, int seconds)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".slow " + seconds.ClampMin(1, 1));
        }

        /// <summary>
        /// Disables slow mode in a room where slow mode is enabled.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put take out of slow mode.</param>
        public void SlowOff(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".slowoff");
        }

        /// <summary>
        /// Puts a room into followers only mode.
        /// Only followers of the channel can speak in chat.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put take out of into followers only mode.</param>
        /// <param name="time">
        /// How long a user has to befflowed in order to talk.
        /// Default is 0 minutes, minumum is 0 minutes, maximum is 3 months (90 days).
        /// Twitch consideres 1 month to be 30 days.
        /// Only the minutes, hours, and days are considered.</param>
        public void FollowersOnly(string room_name, TimeSpan time = default(TimeSpan))
        {
            TimeSpan min = TimeSpan.FromMinutes(0);
            TimeSpan max = TimeSpan.FromDays(90);
            time = time.Clamp(min, max, min);
            string time_string = time.Minutes + " minutes " + time.Hours + " hours " + time.Days + " days";

            EnqueuePrivmsg(room_name.ToLower(), ".followers " + time_string);
        }

        /// <summary>
        /// Disables followers only mode in a room where followers only mode is enabled. 
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to take out of follower only mode.</param>
        public void FollowersOnlyOff(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".followersoff");
        }

        /// <summary>
        /// Puts a room into subscribers only mode.
        /// Only subscribers of the channel can speak in chat.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put into subsribers only mode.</param>
        public void SubscribersOnly(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".subscribers");
        }

        /// <summary>
        /// Disables subscribers only mode in a room where subscribers only mode is enabled. 
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to take out of subsribers only mode.</param>
        public void SubscribersOnlyOff(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".subscribersoff");
        }

        /// <summary>
        /// Clears the chat history for a room.
        /// This can fail if a user has browser add-ons that prevents chat clearing.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to clear the chat history from.</param>
        public void ClearChat(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".clear");
        }

        /// <summary>
        /// Puts a room into R9KBeta mode.
        /// Users are forced to send "unique" messages. 
        /// Twitch will check for a minimum of 9 characters that are not symbol unicode characters and then purges and repetitive chat lines beyond that
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put into R9KBeta mode.</param>
        public void R9KBeta(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".r9kbeta");
        }

        /// <summary>
        /// Disables R9KBeta mode in a room where R9KBeta mode is enabled. 
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to take out of R9KBeta mode.</param>
        public void R9KBetaOff(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".r9kbetaoff");
        }

        /// <summary>
        /// Puts a room into emote only mode.
        /// Users can send messages only containing emotes.
        /// Moderators, the broadcaster, and those who cheer with bits are exempt from this mode.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to put into emote only mode.</param>
        public void EmoteOnly(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".emoteonly");
        }

        /// <summary>
        /// Disables emote only mode in a room where emote only mode is enabled.
        /// The client must be a moderator in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">Room to take out of emote only mode.</param>
        public void EmoteOnlyOff(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".emoteonlyoff");
        }

        #endregion

        #region Editor commands

        /// <summary>
        /// Runs a commercial in a room. The default length is 30 seconds. The client must be an editor in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">The room to run the commercial in.</param>
        /// <param name="length">The length of the commercial. Default is 30 seconds.</param>
        public void Commercial(string room_name, CommercialLength length = CommercialLength.seconds_30)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".commercial " + length.ToString().TextAfter('_'));
        }

        /// <summary>
        /// Host another channel. The client must be an editor in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">The room to send the host command in.</param>
        /// <param name="channel_name">The channel to host.</param>
        public void Host(string room_name, string channel_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".host " + channel_name.ToLower());
        }

        /// <summary>
        /// Unhost any channel that is currently being hosted. The client must be an editor in the room or the broadcaster for the command to succeed.
        /// </summary>
        /// <param name="room_name">The room to send the unhost command in.</param>
        public void Unhost(string room_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".unhost");
        }

        #endregion

        #region Broadcaster commands

        /// <summary>
        /// Mods a user in a specific room. The client must be the broadcaster for the command to succeed.
        /// </summary>
        public void Mod(string room_name, string user_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".mod " + user_name.ToLower());
        }

        /// <summary>
        /// Unmods a user in a specific room. The client must be the broadcaster for the command to succeed unless the client is un-modding itself.
        /// </summary>
        public void Unmod(string room_name, string user_name)
        {
            EnqueuePrivmsg(room_name.ToLower(), ".unmod " + user_name.ToLower());
        }

        #endregion

        #endregion

        #region Sending Twitch chat commands

        /// <summary>
        /// Enqueues a Privmsg to be sent to be sent to a channel/room.
        /// </summary>
        /// <param name="room_name">The room to send the message to.</param>
        /// <param name="message">The message to be sent to the channel/room.</param>
        public void EnqueuePrivmsg(string room_name, string message)
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "PRIVMSG enqueue process starting...");

            if (!room_name.isValid())
            {
                Log.Warning(debug_prefix + "Cannot enqueue PRIVMSG, " + nameof(room_name).Wrap("\"", "\"") + " is empty or null.");
                Log.Error(TimeStamp.TimeLong, debug_prefix + "PRIVMSG enqueue process aborted");
                Log.BlankLine();

                return;
            }

            message = message.RemovePadding();

            if (!message.isValid())
            {
                Log.Warning(debug_prefix + "Cannot enqueue PRIVMSG, " + nameof(message).Wrap("\"", "\"") + " is empty or null.");
                Log.Error(TimeStamp.TimeLong, debug_prefix + "PRIVMSG enqueue process aborted");
                Log.BlankLine();

                return;
            }

            // whispers seem to be treated separately from normal messages and commands, keep them separate
            privmsg_queue.Enqueue(new MessageTemplate
            {
                target = room_name,
                message = message
            });

            Log.PrintLine(debug_prefix + "PRIVMSG successfully enqueued",
                          debug_prefix + Log.FormatColumns(nameof(message), message));
            Log.Header(TimeStamp.TimeLong, debug_prefix + "PRIVMSG enqueue process completed");
            Log.BlankLine();
        }

        /// <summary>
        /// Dequeues and sends a chat command message if any exist.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void ProcressEnqueuedChatCommands(object sender, ElapsedEventArgs e)
        {
            if(privmsg_queue.Count < 1)
            {
                return;
            }

            Log.Header(TimeStamp.TimeLong, debug_prefix + "Dequeuing and sending PRIVMSG...");
            MessageTemplate privmsg = privmsg_queue.Dequeue();
            SendPrivmsg(privmsg.target, privmsg.message);
        }

        /// <summary>
        /// Enqueues a whisper message to be sent to a user.
        /// </summary>
        /// <param name="recipient">The ser who receives the whisper.</param>
        /// <param name="message">The message to be sent to the user.</param>
        public void EnqueueWhisper(string recipient, string message)
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "WHISPER enqueue process starting...");

            if (!recipient.isValid())
            {
                Log.Warning(debug_prefix + "Cannot enqueue WHISPER, " + nameof(recipient).Wrap("\"", "\"") + " is empty or null.");
                Log.Error(TimeStamp.TimeLong, debug_prefix + "WHISPER enqueue process aborted");

                return;
            }

            if (!message.isValid())
            {
                Log.Warning(debug_prefix + "Cannot enqueue WHISPER, " + nameof(message).Wrap("\"", "\"") + " is empty or null.");
                Log.Error(TimeStamp.TimeLong, debug_prefix + "WHISPER enqueue process aborted");

                return;
            }

            // whispers seem to be treated separately from normal messages and commands, keep them separate
            whisper_queue.Enqueue(new MessageTemplate
            {
                target = recipient,
                message = message
            });

            Log.PrintLine(debug_prefix + "WHISPER successfully enqueued",
                          debug_prefix + Log.FormatColumns(nameof(message), message));
            Log.Header(TimeStamp.TimeLong, debug_prefix + "WHISPER enqueue process completed");
            Log.BlankLine();
        }

        /// <summary>
        /// Dequeues and sends a whisper if any exist.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void ProcressEnqueuedWhispers(object sender, ElapsedEventArgs e)
        {
            if (whisper_queue.Count < 1)
            {
                return;
            }

            Log.Header(TimeStamp.TimeLong, debug_prefix + "Dequeuing and sending WHISPER...");
            MessageTemplate whisper = whisper_queue.Dequeue();
            SendWhisper(whisper.target, whisper.message);
        }

        /// <summary>
        /// Sends a whisper message to a user.
        /// Using this method is dangerous and is not throttled.
        /// It is recommended to use <see cref="EnqueueWhisper(string, string)"/> to avoid the chance of getting global banned.
        /// </summary>
        /// <param name="recipient">The ser who receives the whisper.</param>
        /// <param name="message">The message to be sent to the user.</param>
        public void SendWhisper(string recipient, string message)
        {
            Send(string.Format("PRIVMSG #jtv :/w {0} {1}", recipient.ToLower(), message));
        }

        #endregion

        #region Threads

        /*
        private void MonitorNewFollowers()
        {
            List<Follower> launch_followers = twitch_api.GetChannelFollowers(user._id);

            Trie follower_cache = new Trie();
            foreach(Follower follower in launch_followers)
            {
                follower_cache.Insert(follower.user.name);
                LibraryDebug.PrintLine(follower.user.name);
            }

            DateTime updated_at_limit = launch_followers.isValidList() ? launch_followers[0].created_at : DateTime.Now;

            DateTime last_check = DateTime.Now;

            while(isConnected())
            {
                // get new followers once every 5 seconds 
                if(DateTime.Now - last_check < TimeSpan.FromMilliseconds(1000))
                {
                    Thread.Sleep(50);

                    continue;
                }

                last_check = DateTime.Now;

                List<Follower> followers = GetNewFollowers(twitch_api, ref updated_at_limit, ref follower_cache);

                if (!followers.isValidList())
                {
                    continue;
                }

                foreach(Follower follower in followers)
                {
                    OnNewFollower.Raise(this, new OnNewFollowerArgs { follower = follower });
                }

                Thread.Sleep(50);
            }
        }
        */

        #endregion

        #region Services

        /*
        /// <summary>
        /// Gets all of the new followers up until a certain <see cref="DateTime"/>.
        /// </summary>
        private List<Follower> GetNewFollowers(TwitchApiOAuth twitch_api_oauth, ref DateTime updated_at_limit, ref Trie followers_cache)
        {
            bool searching = true;

            List<Follower> followers = new List<Follower>();

            PagingChannelFollowers paging = new PagingChannelFollowers();
            paging.limit = 100;

            FollowerPage follower_page = twitch_api_oauth.GetChannelFollowersPage(user._id, paging);

            do
            {
                foreach (Follower follower in follower_page.follows)
                {
                    // the date followed is equal to or earlier than the date of the most recent recorded follower, guaranteed no new followers passed this point 
                    if (DateTime.Compare(follower.created_at, updated_at_limit) <= 0)
                    {
                        updated_at_limit = follower.created_at;
                        searching = false;

                        break;
                    }

                    if (followers_cache.Insert(follower.user.display_name))
                    {
                        followers.Add(follower);
                    }
                }

                if (follower_page._cursor.isValidString())
                {
                    if (searching)
                    {
                        paging.cursor = follower_page._cursor;

                        follower_page = twitch_api_oauth.GetChannelFollowersPage(user._id, paging);
                    }
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            // update the new updated_at limit to check to the newest user that followed 
            if (followers.isValidList())
            {
                updated_at_limit = followers[0].created_at;
            }

            return followers;
        }
        */

        #endregion
    }
}