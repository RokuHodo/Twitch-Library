using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;


//project namespaces
using TwitchLibrary.API;
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Models.Messages.Subscriber;
using TwitchLibrary.Models.Messages.Private;
using TwitchLibrary.Models.Messages.Whisper;

namespace TwitchLibrary.Clients
{
    public class TwitchClient
    {
        readonly int G_PORT = 6667;

        readonly string G_IP_ADRESS = "irc.chat.twitch.tv";

        private bool g_reading;

        private string G_OAUTH_TOKEN;

        private TcpClient g_tcp_Client;

        private StreamReader g_reader;
        private StreamWriter g_writer;

        private Thread MonitorIrcMessages_Thread;

        public int _id;

        public string name,
                      display_name;

        public event EventHandler<OnReSubscriberArgs> OnReSubscriber;
        public event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;
        public event EventHandler<OnPingReceivedArgs> OnPingReceived;
        public event EventHandler<OnIrcMessageReceivedArgs> OnIrcMessageReceived;
        public event EventHandler<OnNullIrcMessageReceivedArgs> OnNullIrcMessageReceived;
        public event EventHandler<OnPrivateMessageReceivedArgs> OnPrivateMessageReceived;
        public event EventHandler<OnWhisperMessageReceivedArgs> OnWhisperMessageReceived;

        public TwitchClient(string oauth_token)
        {
            G_OAUTH_TOKEN = oauth_token;
            User user = new TwitchApiOAuth(G_OAUTH_TOKEN).GetUser();

            _id = user._id;
            name = user.name;
            display_name = user.display_name;

            MonitorIrcMessages_Thread = new Thread(new ThreadStart(MonitorIrcMessages));

            Connect();
        }

        #region Threads

        /// <summary>
        /// Reads messages sent from the IRC and passes them to the event handler.
        /// </summary>
        private void MonitorIrcMessages()
        {
            try
            {
                while (isConnected() && g_reading)
                {
                    string message = g_reader.ReadLine();

                    //probably got disconnected by the server, reconnect
                    if (!message.isValidString())
                    {                        
                        LibraryDebug.Error(name.Wrap("[ ", " ]") + " null message recieved from the IRC connection. Auto-reconnecting...", TimeStamp.TimeLong);
                        Reconnect();                      

                        OnNullIrcMessageReceived.Raise(this, new OnNullIrcMessageReceivedArgs { time = DateTime.Now });

                        continue;
                    }

                    if (message.StartsWith("PING"))
                    {
                        LibraryDebug.Notify(name.Wrap("[ ", " ]") + " " + message.Wrap("\"", "\"") + " recieved from the IRC connection, Auto-responded with \"PONG\"", TimeStamp.TimeLong);
                        SendPong();                     

                        OnPingReceived.Raise(this, new OnPingReceivedArgs { ping_message = message });

                        continue;
                    }

                    LibraryDebug.PrintLine(message);                                        
 
                    IrcMessage irc_message = new IrcMessage(message);
                    OnIrcMessageReceived.Raise(this, new OnIrcMessageReceivedArgs { irc_message = irc_message });     
                    
                    switch(irc_message.command)
                    {
                        case "PRIVMSG":
                            {
                                PrivateMessage private_message = new PrivateMessage(irc_message, G_OAUTH_TOKEN);
                                if (private_message.sender.name == "twitchnotify")
                                {
                                    if (private_message.body.IndexOf("just subscribed") != -1)
                                    {
                                        NewSubcriberMessage subscriber_message = new NewSubcriberMessage(private_message);
                                        OnNewSubscriber.Raise(this, new OnNewSubscriberArgs { subscriber_message = subscriber_message });
                                    }
                                }
                                else
                                {                                    
                                    OnPrivateMessageReceived.Raise(this, new OnPrivateMessageReceivedArgs { private_message = private_message });
                                }                                    
                            }
                            break;
                        case "WHISPER":
                            {
                                WhisperMessage whisper_message = new WhisperMessage(irc_message, G_OAUTH_TOKEN);
                                OnWhisperMessageReceived.Raise(this, new OnWhisperMessageReceivedArgs { whisper_message = whisper_message });
                            }
                            break;
                        case "USERNOTICE":
                            {
                                ReSubscriberMessage subscriber_message = new ReSubscriberMessage(irc_message, G_OAUTH_TOKEN);
                                OnReSubscriber.Raise(this, new OnReSubscriberArgs { subscriber_message = subscriber_message });
                            }
                            break;
                    }
                }
            }
            catch(ThreadAbortException exception)
            {
                LibraryDebug.Warning(nameof(MonitorIrcMessages_Thread) + " abort called.");
                LibraryDebug.PrintLine(nameof(exception), exception.Message);                
            }

            Disconnect();
        }

        #endregion

        #region Connect, disconnect, reconnect, and connection handling

        /// <summary>
        /// Connect to the IRC server
        /// </summary>
        public void Connect()
        {
            LibraryDebug.BlankLine();
            LibraryDebug.Notify(name.Wrap("[ ", " ]") + " Connecting to Twitch...", TimeStamp.TimeLong);

            g_tcp_Client = new TcpClient(G_IP_ADRESS, G_PORT);            

            //create the reader/wrtier to communicate with the irc
            g_reader = new StreamReader(g_tcp_Client.GetStream());
            g_writer = new StreamWriter(g_tcp_Client.GetStream());

            if(MonitorIrcMessages_Thread.ThreadState != ThreadState.Running)
            {
                g_reading = true;
                MonitorIrcMessages_Thread.Start();
            }            

            g_writer.AutoFlush = true;

            //log into the irc
            g_writer.WriteLine("PASS oauth:" + G_OAUTH_TOKEN);
            g_writer.WriteLine("NICK " + name);
            g_writer.WriteLine("USER " + name + " 8 * :" + name);

            //request to recieve notices and user information through the irc
            g_writer.WriteLine("CAP REQ :twitch.tv/tags");
            g_writer.WriteLine("CAP REQ :twitch.tv/membership");
            g_writer.WriteLine("CAP REQ :twitch.tv/commands");

            g_writer.Flush();
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            LibraryDebug.Notify(name.Wrap("[ ", " ]") + " Disconnecting from Twitch...", TimeStamp.TimeLong);

            g_tcp_Client.Close();
            
            g_reader.DiscardBufferedData();
            g_reader.Dispose();
            g_reader.Close();

            if(MonitorIrcMessages_Thread.ThreadState != ThreadState.Aborted || MonitorIrcMessages_Thread.ThreadState != ThreadState.AbortRequested)
            {
                MonitorIrcMessages_Thread.Abort();
            }            

            g_writer.Flush();
            g_writer.Dispose();
            g_writer.Close();
        }

        /// <summary>
        /// Reconnect to the IRC server
        /// </summary>
        public void Reconnect()
        {
            LibraryDebug.Notify(name.Wrap("[ ", " ]") + " Reconnecting to Twitch...", TimeStamp.TimeLong);

            Disconnect();
            Connect();
        }

        /// <summary>
        /// Determines if the <see cref="TcpClient"/> is connected to the server
        /// /// </summary>
        public bool isConnected()
        {
            return g_tcp_Client.Connected;
        }        

        /// <summary>
        /// Sends "pong" to the irc connection when to stay connected.
        /// </summary>
        public void SendPong()
        {
            if(!isConnected())
            {
                return;
            }

            g_writer.WriteLine("PONG");
            g_writer.Flush();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Purges a user for 1 second.
        /// </summary>
        public void Purge(string user, string reason = "")
        {
            Timeout(user, 1, reason);
        }

        /// <summary>
        /// Times out a user for a specified amount of time with an optional reason.
        /// </summary>
        public void Timeout(string user, int seconds, string reason = "")
        {
            g_writer.WriteLine("PRIVMSG #{0} :{1}", name, ".timeout " + user.ToLower() + " " + seconds.ToString() + " " + reason);
        }

        /// <summary>
        /// Bans a user.
        /// </summary>
        public void Ban(string user, string reason = "")
        {
            g_writer.WriteLine("PRIVMSG #{0} :{1}", name, "/ban " + user.ToLower() + " " + reason);
        }

        /// <summary>
        /// Unbans a user.
        /// </summary>
        public void Unban(string user)
        {
            g_writer.WriteLine("PRIVMSG #{0} :{1}", name, "/unban " + user.ToLower());
        }

        /// <summary>
        /// Mods a user.
        /// </summary>
        public void Mod(string user)
        {
            g_writer.WriteLine("PRIVMSG #{0} :{1}", name, "/mod " + user.ToLower());
        }

        /// <summary>
        /// Unmods a user.
        /// </summary>
        public void Unmod(string user)
        {
            g_writer.WriteLine("PRIVMSG #{0} :{1}", name, "/unmod " + user.ToLower());
        }        

        /// <summary>
        /// Join a channel's room.
        /// </summary>        
        public void Join(string channel)
        {
            channel = channel.ToLower();

            LibraryDebug.BlankLine();
            LibraryDebug.Notify("Joining room: " + channel, TimeStamp.TimeLong);

            g_writer.WriteLine("JOIN #" + channel);
            g_writer.Flush();
        }

        /// <summary>
        /// Leave a channel's room.
        /// </summary>        
        public void Leave(string channel)
        {
            channel = channel.ToLower();

            LibraryDebug.BlankLine();
            LibraryDebug.Notify("Leaving room: " + channel, TimeStamp.TimeLong);

            g_writer.WriteLine("PART #" + channel);
            g_writer.Flush();
        }

        #endregion

        #region Send messages and whispers

        /// <summary>
        /// Sends a message to the current chat room.
        /// </summary>
        public void SendMessage(string room, string message)
        {
            if (!room.isValidString() || !message.isValidString() || !isConnected())
            {
                return;
            }

            g_writer.WriteLine(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{1} :{2}", name, room.ToLower(), message);
            g_writer.Flush();
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

            g_writer.WriteLine("PRIVMSG #jtv :/w {0} {1}", recipient.ToLower(), message);
            g_writer.Flush();
        }

        #endregion
    }
}