//standard namespaces
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients.IRC;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.Messages.Subscriber;
using TwitchLibrary.Models.Messages.Whisper;

//imported .dll's
using ChatSharp;
using ChatSharp.Events;


//TODO: (IRC) Test to see if ChatSharp handles ping and disconnects and address them accordingly

namespace TwitchLibrary.Clients.IRC
{
    public class TwitchIrcClient
    {
        //private
        private int PORT = 6667;

        private string IP_ADRESS = "irc.chat.twitch.tv";
        private string OAUTH_TOKEN;

        private User user;
        private TwitchApiOAuth twitch_api;

        private IrcUser irc_user;
        private IrcClient irc_client;

        private List<string> names;

        //public
        public string _id;
        public string name;
        public string display_name;

        public event EventHandler<EventArgs> OnConnect;

        //TODO: (IRC) Implement follower service and OnNewFollower
        //public event EventHandler<OnNewFollowerEventArgs> OnNewFollower;

        public event EventHandler<NamesReceivedEventArgs> OnNamesReceived;

        public event EventHandler<UseResubscriberEventArgs> OnUserResubscribed;
        public event EventHandler<UserSubscribedEventArgs> OnUserSubscribed;

        //message events
        public event EventHandler<PrivateMessageReceivedEventArgs> OnPrivateMessageReceived;
        public event EventHandler<WhisperMessageReceivedEventArgs> OnWhisperMessageReceived;
        public event EventHandler<UnsupportedMessageReceivedEventArgs> OnUnsupportedMessageReceived;

        public TwitchIrcClient(string oauth_token)
        {
            OAUTH_TOKEN = oauth_token;
            twitch_api = new TwitchApiOAuth(OAUTH_TOKEN);
            user = twitch_api.GetUser();

            _id = user._id;
            name = user.name;
            display_name = user.display_name;

            irc_user = new IrcUser(name, name, "oauth:" + oauth_token);
            irc_client = new IrcClient(IP_ADRESS + ":" + PORT, irc_user);            
            irc_client.ConnectionComplete += new EventHandler<EventArgs>(OnConnectionComplete);
            irc_client.RawMessageRecieved += new EventHandler<RawMessageEventArgs>(OnIrcMessageReceived);

            //TODO: (IRC) Have the user connect themselves.
            irc_client.ConnectAsync();              
        }

        //TODO: (IRC) Connect().
        //TODO: (IRC) ConnectAsync().
        //TODO: (IRC) Reonnect().
        //TODO: (IRC) ReconnectAsync().
        //TODO: (IRC) Disconnect().
        //TODO: (IRC) DisconnectAsync().

        private void OnConnectionComplete(object sender, EventArgs e)
        {   
            //TODO: (IRC) Give the user an option to request tags/membership/commands or not
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/tags");
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/membership");
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/commands");

            //TODO: (IRC) Modify OnSuccessfulConnection
            OnConnect.RaiseAsync(this, new EventArgs());            
        }

        private void OnIrcMessageReceived(object sender, RawMessageEventArgs e)
        {
            //use our IrcMessage because ChatSharp can't successfully parse the raw message, Twitch never sent '005' while logging in
            //TODO: (IRC) OnIrcMessageReceived
            Models.Messages.IRC.IrcMessage irc_message = new Models.Messages.IRC.IrcMessage(e.Message);

            switch (irc_message.command)
            {
                case "PRIVMSG":
                    {
                        //NOTE: (IRC) IRC Command - PRIVMSG tags only sent when / tags is requested
                        Models.Messages.Private.PrivateMessage private_message = new Models.Messages.Private.PrivateMessage(irc_message, OAUTH_TOKEN);

                        if (private_message.sender.name == "twitchnotify")
                        {
                            if (private_message.body.IndexOf("just subscribed") != -1)
                            {
                                UserSubcribedMessage subscriber_message = new UserSubcribedMessage(private_message);
                                OnUserSubscribed.RaiseAsync(this, new UserSubscribedEventArgs
                                {
                                    subscriber_message = subscriber_message
                                });
                            }
                        }
                        else
                        {
                            OnPrivateMessageReceived.RaiseAsync(this, new PrivateMessageReceivedEventArgs
                            {
                                private_message = private_message
                            });
                        }
                    }
                    break;
                case "WHISPER":
                    {
                        WhisperMessage whisper_message = new WhisperMessage(irc_message, OAUTH_TOKEN);
                        OnWhisperMessageReceived.RaiseAsync(this, new WhisperMessageReceivedEventArgs
                        {
                            whisper_message = whisper_message
                        });
                    }
                    break;
                case "USERNOTICE":
                    {
                        //NOTE: (IRC) IRC Command - USERNOTICE requires /commands
                        //NOTE: (IRC) IRC Command - USERNOTICE tags only sent when /tags is requested
                        ResubscriberMessage subscriber_message = new ResubscriberMessage(irc_message, OAUTH_TOKEN);
                        OnUserResubscribed.RaiseAsync(this, new UseResubscriberEventArgs
                        {
                            subscriber_message = subscriber_message
                        });
                    }
                    break;
                case "JOIN":
                    {
                        //NOTE: (IRC) IRC Command - JOIN requires /membership

                        //TODO: (IRC) IRC Command - JOIN, OnUserJoinedReceived

                    }
                    break;
                case "PART":
                    {
                        //NOTE: (IRC) IRC Command - PART requires /membership

                        //TODO: (IRC) IRC Command - PART, OnUserPartReceived
                    }
                    break;
                case "MODE":
                    {
                        //NOTE: (IRC) IRC Command - MODE requires /membership

                        //NOTE: (IRC) IRC Command - MODE, OnUserModded EXTEMELY unreliable

                        //TODO: (IRC) IRC Command - MODE, OnUserModdedReceived

                        //NOTE: (IRC) IRC Command - MODE, OnUserUnmodded EXTEMELY unreliable

                        //TODO: (IRC) IRC Command - MODE, OnUserUnmoddedReceived
                    }
                    break;
                case "353":
                    {
                        //NOTE: (IRC) IRC Command - 353 (NAMES) requires /membership, will only list OP users is number of users in the room > 1000
                        if (!names.isValid())
                        {
                            names = new List<string>();
                        }

                        foreach(string name in irc_message.trailing)
                        {
                            names.Add(name);
                        }
                    }
                    break;
                case "366":
                    {
                        //NOTE: (IRC) IRC Command - 366 (NAMES) requires /membership
                        //NOTE: (IRC) IRC Command - 366 (NAMES) once this line is recieved, the stored names from 353 should be raised and then emptied for the next 353
                        OnNamesReceived.RaiseAsync(this, new NamesReceivedEventArgs
                        {
                            names = names
                        });

                        names = new List<string>();
                    }
                    break;
                case "CLEARCHAT":
                    {
                        //NOTE: (IRC) IRC Command - CLEARCHAT requires /commands
                        //NOTE: (IRC) IRC Command - CLEARCHAT tags only sent when /tags is requested

                        //TODO: (IRC) IRC Command - CLEARCHAT, OnChatClearedReceived
                    }
                    break;
                case "GLOBALUSERSTATE":
                    {
                        //NOTE: (IRC) IRC Command - GLOBALUSERSTATE tags only sent when /tags is requested
                        //NOTE: (IRC) IRC Command - GLOBALUSERSTATE is possibly already handled by ChatSharp, may not need this or add independent event

                        //TODO: (IRC) IRC Command - GLOBALUSERSTATE, OnGlobalUserstateReceived/OnSuccessfulLogin?
                    }
                    break;
                case "ROOMSTATE":
                    {
                        //NOTE: (IRC) IRC Command - ROOMSTATE requires /commands
                        //NOTE: (IRC) IRC Command - ROOMSTATE tags only sent when /tags is requested
                        //NOTE: (IRC) IRC Command - ROOMSTATE is sent when a user joins a channel room with all settings in the tag
                        //NOTE: (IRC) IRC Command - ROOMSTATE is sent when a room setting is changed only with the relevant tag
                        
                        //TODO: (IRC) IRC Command - ROOMSTATE, OnRoomStateReceived
                    }
                    break;
                case "USERSTATE ":
                    {
                        //NOTE: (IRC) IRC Command - USERSTATE requires /commands
                        //NOTE: (IRC) IRC Command - USERSTATE tags only sent when /tags is requested
                        //NOTE: (IRC) IRC Command - USERSTATE is sent when a user joins a channel room just like ROOMSTATE
                        //NOTE: (IRC) IRC Command - USERSTATE is also sent a user sends a message in a channel room but with different tags

                        //TODO: (IRC) IRC Command - USERSTATE, OnUserStateReceived
                    }
                    break;
                case "RECONNECT":
                    {
                        //NOTE: (IRC) IRC Command - RECONNECT requires /commands
                        //NOTE: (IRC) IRC Command - RECONNECT when this is received from the server, the client needs to recconnect

                        //TODO: (IRC) IRC Command - RECONNECT, OnReconnectReceived
                    }
                    break;
                case "NOTICE":
                    {
                        //NOTE: (IRC) IRC Command - NOTICE requires /commands

                        //TODO: (IRC) IRC Command - NOTICE, OnNoticeReceived
                    }
                    break;
                case "HOSTTARGET":
                    {
                        //NOTE: (IRC) IRC Command - HOSTTARGET requires /commands
                        //NOTE: (IRC) IRC Command - HOSTTARGET is sent when the CLIENT either hosts or unhosts someone, not when the client is hosted (i think)

                        //TODO: (IRC) IRC Command - HOSTTARGET, OnHostTargetReceived
                    }
                    break;
                default:
                    {
                        OnUnsupportedMessageReceived.RaiseAsync(this, new UnsupportedMessageReceivedEventArgs
                        {
                            message = e.Message,
                            irc_message = irc_message
                        });
                    }
                    break;

            }
        }

        private void ClearNames(TaskStatus status)
        {
            LibraryDebug.PrintLine("Called back");
            names = new List<string>();
        }

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
                //get new followers once every 5 seconds 
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

        #region Commands

        //TODO: (IRC) Chat Command - Host and OnChannelHosted
        //TODO: (IRC) Chat Command - Unhost and OnChannelUnhosted
        //TODO: (IRC) Chat Command - StartCommercial and OnCommercialStarted
        //TODO: (IRC) Chat Command - Omoteonly and OnOmoteonly
        //TODO: (IRC) Chat Command - OmoteonlyOff and OnOmoteonlyOff
        //TODO: (IRC) Chat Command - R9KBeta and OnR9KBeta
        //TODO: (IRC) Chat Command - R9KBetaOff and OnR9KBetaOff
        //TODO: (IRC) Chat Command - ClearChat and OnChatCleared
        //TODO: (IRC) Chat Command - SubscribersOnly and OnSubscribersOnly
        //TODO: (IRC) Chat Command - SubscribersOnlyOff and OnSubscribersOnlyOff
        //TODO: (IRC) Chat Command - FollowersOnly and OnFollowersOnly
        //TODO: (IRC) Chat Command - FollowersOnlyOff and OnFollowersOnlyOff
        //TODO: (IRC) Chat Command - Slow and OnSlow
        //TODO: (IRC) Chat Command - SlowOff and OnSlowOff
        //TODO: (IRC) Chat Command - DisconnectChat and OnDisconnectChat
        //TODO: (IRC) Chat Command - Me and OnMe
        //TODO: (IRC) Chat Command - IgnoreUser and OnUserIgnored
        //TODO: (IRC) Chat Command - UnignoreUser and OnUserUnignored
        //TODO: (IRC) Chat Command - Color and OnColorChanged

        /// <summary>
        /// Purges a user in the client's room for 1 second with an optional reason.
        /// </summary>
        public void Purge(string user, string reason = "")
        {
            Timeout(name, user, 1, reason);
        }

        /// <summary>
        /// Purges a user in a specific room for 1 second with an optional reason.
        /// </summary>
        public void Purge(string room_name, string user_name, string reason = "")
        {
            //TODO: (IRC) Chat Command - OnUserPurged
            Timeout(room_name, user_name, 1, reason);
        }

        /// <summary>
        /// Times out a user in the client's room for a specified amount of time with an optional reason.
        /// </summary>
        public void Timeout(string user_name, int seconds, string reason = "")
        {            
            Timeout(name, user_name, seconds, reason);
        }

        /// <summary>
        /// Times out a user in a specific room for a specified amount of time with an optional reason.
        /// </summary>
        public void Timeout(string room_name, string user_name, int seconds, string reason = "")
        {
            //TODO: (IRC) Chat Command - OnUserTimedOut
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2} {3} {4}", room_name.ToLower(), ".timeout", user_name.ToLower(), seconds, reason);
        }

        /// <summary>
        /// Bans a user in the client's room with an optional reason.
        /// </summary>
        public void Ban(string user_name, string reason = "")
        {
            Ban(name, user_name, reason);
        }

        /// <summary>
        /// Bans a user in a specific room with an optional reason
        /// </summary>
        public void Ban(string room_name, string user_name, string reason = "")
        {
            //TODO: (IRC) Chat Command - OnUserBanned
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2} {3}", room_name.ToLower(), ".ban", user_name.ToLower(), reason);
        }

        /// <summary>
        /// Unbans a user in the client's room.
        /// </summary>
        public void Unban(string user_name)
        {
            Unban(name, user_name);
        }

        /// <summary>
        /// Unbans a user in a specific room.
        /// </summary>
        public void Unban(string room_name, string user_name)
        {
            //TODO: (IRC) Chat Command - OnUserUnbanned
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2}", room_name.ToLower(), ".unban", user_name.ToLower());
        }

        /// <summary>
        /// Mods a user in the client's room.
        /// </summary>
        public void Mod(string user_name)
        {            
            Mod(name, user_name);
        }

        /// <summary>
        /// Mods a user in a specific room.
        /// </summary>
        public void Mod(string room_name, string user_name)
        {
            //TODO: (IRC) Chat Command - OnUserModded
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2}", room_name.ToLower(), ".mod", user_name.ToLower());
        }

        /// <summary>
        /// Unmods a user in the client's room.
        /// </summary>
        public void Unmod(string user_name)
        {
            Unmod(name, user_name); 
        }

        /// <summary>
        /// Unmods a user in a specific room.
        /// </summary>
        public void Unmod(string room_name, string user_name)
        {
            //TODO: (IRC) Chat Command - OnUserUnmodded
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2}", room_name, ".unmod", user_name.ToLower());
        }

        /// <summary>
        /// Join a channel's room.
        /// </summary>        
        public void Join(string channel)
        {
            channel = channel.ToLower();

            //TODO: (IRC) Chat Command - OnJoinedChannel
            irc_client.JoinChannel("#" + channel);            

            LibraryDebug.Notify("Joining room: " + channel, TimeStamp.TimeLong);
        }

        /// <summary>
        /// Leave a channel's room.
        /// </summary>        
        public void Part(string channel)
        {
            channel = channel.ToLower();

            //TODO: (IRC) Chat Command - OnPartChannel
            irc_client.PartChannel("#" + channel);

            LibraryDebug.Notify("Leaving room: " + channel, TimeStamp.TimeLong);
        }
        
        #endregion

        #region Send messages and whispers
        /*
        /// <summary>
        /// Sends a message to the current chat room.
        /// </summary>
        public void SendMessage(string room, string message)
        {
            if (!room.isValidString() || !message.isValidString() || !isConnected())
            {
                return;
            }

            writer.WriteLine(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{1} :{2}", name, room.ToLower(), message);
            writer.Flush();
        }

        /// <summary>
        /// Sends a whisper to a specified user.
        /// </summary>
        public void SendWhisper(string recipient, string message)
        {
            if (!message.isValidString() || !isConnected())
            {
                return;
            }

            writer.WriteLine("PRIVMSG #jtv :/w {0} {1}", recipient.ToLower(), message);
            writer.Flush();
        }
        */

        #endregion
        
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
                    //the date followed is equal to or earlier than the date of the most recent recorded follower, guaranteed no new followers passed this point 
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

            //update the new updated_at limit to check to the newest user that followed 
            if (followers.isValidList())
            {
                updated_at_limit = followers[0].created_at;
            }

            return followers;
        }
        */
    }
}