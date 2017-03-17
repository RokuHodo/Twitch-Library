using System;
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Helpers;
using TwitchLibrary.Helpers.Paging.Channels;
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.Messages.Subscriber;
using TwitchLibrary.Models.Messages.Whisper;

//imported .dll's
using ChatSharp;
using ChatSharp.Events;

/*
 * TODO: (Client) Master todo list
 *      -   Finish adding all Twitch IRC commands
 *      -   Add events for each Twitch IRC command
 *      -   Test to see if ChatSharp handles ping and disconnects and address them accordingly
 *      -   Re-implement detecting new followers, possibly as it's own stand alone service but automatically included in the client 
 */

namespace TwitchLibrary.Clients
{
    public class TwitchClient
    {
        //private
        private readonly int PORT = 6667;

        private readonly string IP_ADRESS = "irc.chat.twitch.tv";
        private string OAUTH_TOKEN;

        private User user;
        private TwitchApiOAuth twitch_api;

        private IrcUser irc_user;
        private IrcClient irc_client;

        //public
        public string _id;
        public string name;
        public string display_name;

        public event EventHandler<OnSuccessfulConnectionEventArgs> OnSuccessfulConnection;

        //TODO: reimplement new follower service
        //public event EventHandler<OnNewFollowerEventArgs> OnNewFollower;

        public event EventHandler<OnReSubscriberEventArgs> OnReSubscriber;
        public event EventHandler<OnNewSubscriberEventArgs> OnNewSubscriber;

        public event EventHandler<OnPrivateMessageReceivedEventArgs> OnPrivateMessageReceived;
        public event EventHandler<OnWhisperMessageReceivedEventArgs> OnWhisperMessageReceived;

        public TwitchClient(string oauth_token)
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
            irc_client.ConnectAsync();              
        }

        private void OnConnectionComplete(object sender, EventArgs e)
        {   
            //request message tags, memberhip, and enable custom commands
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/tags");
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/membership");
            irc_client.SendRawMessage("CAP REQ :{0}", "twitch.tv/commands");

            OnSuccessfulConnection.RaiseAsync(this, new OnSuccessfulConnectionEventArgs { });            
        }

        private void OnIrcMessageReceived(object sender, RawMessageEventArgs e)
        {
            //use our IrcMessage because ChatSharp can't successfully parse the raw message, Twitch never sent '005' while logging in
            //TODO: OnIrcMessageReceived
            Models.Messages.IRC.IrcMessage irc_message = new Models.Messages.IRC.IrcMessage(e.Message);

            switch (irc_message.command)
            {
                case "PRIVMSG":
                    {
                        Models.Messages.Private.PrivateMessage private_message = new Models.Messages.Private.PrivateMessage(irc_message, OAUTH_TOKEN);

                        if (private_message.sender.name == "twitchnotify")
                        {
                            if (private_message.body.IndexOf("just subscribed") != -1)
                            {
                                NewSubcriberMessage subscriber_message = new NewSubcriberMessage(private_message);
                                OnNewSubscriber.RaiseAsync(this, new OnNewSubscriberEventArgs { subscriber_message = subscriber_message });
                            }
                        }
                        else
                        {
                            OnPrivateMessageReceived.RaiseAsync(this, new OnPrivateMessageReceivedEventArgs { private_message = private_message });
                        }
                    }
                    break;
                case "WHISPER":
                    {
                        WhisperMessage whisper_message = new WhisperMessage(irc_message, OAUTH_TOKEN);
                        OnWhisperMessageReceived.RaiseAsync(this, new OnWhisperMessageReceivedEventArgs { whisper_message = whisper_message });
                    }
                    break;
                case "USERNOTICE":
                    {
                        ReSubscriberMessage subscriber_message = new ReSubscriberMessage(irc_message, OAUTH_TOKEN);
                        OnReSubscriber.RaiseAsync(this, new OnReSubscriberEventArgs { subscriber_message = subscriber_message });
                    }
                    break;
            }
        }

        #region Threads

        /*
        private void MonitorNewFollowers()
        {
            //TODO: make this in it's own separate async function, and just make this less hacky in general
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
            //TODO: OnPurge
            Timeout(room_name, user_name, 1, reason);
        }

        /// <summary>
        /// Times out a user in the client's room for a specified amount of time with an optional reason.
        /// </summary>
        public void Timeout(string user_name, int seconds, string reason = "")
        {
            //TODO: OnTimeout
            Timeout(name, user_name, seconds, reason);
        }

        /// <summary>
        /// Times out a user in a specific room for a specified amount of time with an optional reason.
        /// </summary>
        public void Timeout(string room_name, string user_name, int seconds, string reason = "")
        {
            //TODO: OnTimeout
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
            //TODO: OnBan
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
            //TODO: OnUnBan
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
            //TODO: OnMod
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
            //TODO: OnUnmod            
            irc_client.SendRawMessage("PRIVMSG #{0} :{1} {2}", room_name, ".unmod", user_name.ToLower());
        }

        /// <summary>
        /// Join a channel's room.
        /// </summary>        
        public void Join(string channel)
        {
            channel = channel.ToLower();

            //TODO: OnJoinedChannel
            irc_client.JoinChannel("#" + channel);            

            LibraryDebug.BlankLine();
            LibraryDebug.Notify("Joining room: " + channel, TimeStamp.TimeLong);
        }

        /// <summary>
        /// Leave a channel's room.
        /// </summary>        
        public void Part(string channel)
        {
            channel = channel.ToLower();

            //TODO: OnPartChannel
            irc_client.PartChannel("#" + channel);

            LibraryDebug.BlankLine();
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
    }
}