﻿// standard namespaces
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Models.Messages.IRC;
using TwitchLibrary.Enums.Clients.IRC;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients.IRC;
using TwitchLibrary.Events.Clients.IRC.Commands.Native;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;

namespace TwitchLibrary.Clients.IRC
{
    public class IrcClient
    {
        #region Fields

        // Private

        private bool                                    reconnecting;

        private int                                     port;

        private string                                  host;
        private string                                  debug_prefix;

        private Dictionary<string, List<string>>        names;

        private Socket                                  socket;
        private NetworkStream                           stream;

        private Thread                                  reader_thread;

        private IrcUser                                 user;

        private Encoding                                encoding;

        // Public

        /// <summary>
        /// If set to true, the client will automatically respond with a PONG when a PING is receieved from the IRC.
        /// </summary>
        public bool                                     auto_pong;

        /// <summary>
        /// Raised when PING is received from the IRC.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs>  OnPing;        
        
        /// <summary>
        /// Raised when the client successfully connects to the IRC.
        /// </summary>
        public event EventHandler<EventArgs>            OnConnected;

        /// <summary>
        /// Raised when the client successfully reconnects to the IRC.
        /// </summary>
        public event EventHandler<EventArgs>            OnReconnected;        

        /// <summary>
        /// Raised when the client successfully disconnects from the IRC.
        /// </summary>
        public event EventHandler<EventArgs>            OnDisconnected;

        /// <summary>
        /// Raised when a user joins a channel room.
        /// </summary>
        public event EventHandler<JoinEventArgs>        OnJoin;

        /// <summary>
        /// Raised when a user leaves a channel room.
        /// </summary>
        public event EventHandler<PartEventArgs>        OnPart;

        /// <summary>
        /// Raised when a user joins a channel room.
        /// </summary>
        public event EventHandler<ModeEventArgs>        OnMode;

        /// <summary>
        /// Raised when the names (366) is received from the IRC.
        /// </summary>
        public event EventHandler<NamesEventArgs>       OnNames;

        /// <summary>
        /// Raised when an irc message has been received.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs>  OnIrcMessage;

        /// <summary>
        /// Raised when a private message has been received in a channel that the client has joined.
        /// </summary>
        public event EventHandler<PrivmsgEventArgs>     OnPrivmsg;

        /// <summary>
        /// Raised when a message is sent to the IRC.
        /// </summary>
        public event EventHandler<MessageEventArgs>     OnMessageSent;

        /// <summary>
        /// Raised when an unsupported IRC command has been received and cannot be processed by the library.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs>  OnUnsupportedCommand;

        #endregion

        #region Properties

        /// <summary>
        /// The current state of the client.
        /// </summary>
        public TwitchClientState state { get; private set; }

        #endregion

        #region Constructors

        public IrcClient(string host, int port, IrcUser user)
        {
            this.user = user;

            this.port = port;
            this.host = host;

            debug_prefix = "IrcClint " + user.nick.Wrap("\"", "\"") + " - ";

            reconnecting = false;
            auto_pong = true;

            encoding = Encoding.UTF8;

            names = new Dictionary<string, List<string>>();

            state = TwitchClientState.Disconnected;
        }

        #endregion

        #region Connection methods

        /// <summary>
        /// Connects to the IRC.
        /// </summary>
        public void Connect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Connection process starting...");

            if (!CanConnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Connection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            state = TwitchClientState.Connecting;

            try
            {
                LibraryDebug.PrintLine(debug_prefix + "Connecting to socket...",
                                       LibraryDebug.FormatAsColumns(nameof(host), host),
                                       LibraryDebug.FormatAsColumns(nameof(port), port.ToString()));

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);

                LibraryDebug.PrintLine(debug_prefix + "Connected to socket");

                CompleteSocketConnect();
            }
            catch(Exception exception)
            {
                LibraryDebug.PrintLine(debug_prefix + "Failed to connecting to socket",
                                       LibraryDebug.FormatAsColumns(nameof(exception), exception.Message));
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Connection process aborted");
                LibraryDebug.BlankLine();

                socket.Close();
                socket.Dispose();

                state = TwitchClientState.Disconnected;
            }
        }

        /// <summary>
        /// Asynchronously connects to the IRC.
        /// </summary>
        public void ConnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Async connection process starting...");

            if (!CanConnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Async connection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            state = TwitchClientState.Connecting;

            LibraryDebug.PrintLine(debug_prefix + "Asynchronously connecting to socket...",
                                   LibraryDebug.FormatAsColumns(nameof(host), host),
                                   LibraryDebug.FormatAsColumns(nameof(port), port.ToString()));

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(host, port, Callback_OnSocketConnected, null);
        }

        /// <summary>
        /// Fired when the socket has connected to the IRC, successful or not.
        /// Handles completing the connection if successful or aborting the process if it failed.
        /// </summary>
        /// <param name="result">The async result of operation.</param>
        private void Callback_OnSocketConnected(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                LibraryDebug.PrintLine(debug_prefix + "Asynchronously connected to socket");

                CompleteSocketConnect();
            }
            else
            {
                stream.Close();
                stream.Dispose();

                socket.Close();
                socket.Dispose();

                LibraryDebug.PrintLine(debug_prefix + "Failed to asynchronously connect to socket");
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Async connection process aborted");
                LibraryDebug.BlankLine();

                state = TwitchClientState.Disconnected;
            }
        }

        /// <summary>
        /// Starts the stream reader thread and signs into to the IRC.
        /// </summary>
        private void CompleteSocketConnect()
        {
            reader_thread = new Thread(new ThreadStart(ReadStream));
            reader_thread.Start();

            stream = new NetworkStream(socket);

            LibraryDebug.PrintLine(debug_prefix + "Signing into IRC " + host.Wrap("\"", "\""));

            Send("PASS oauth:" + user.pass);
            Send("NICK " + user.nick);
        }

        /// <summary>
        /// Checks to see if it is safe to connect to the IRC.
        /// </summary>
        private bool CanConnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot connect to " + host.Wrap("\"", "\"") +", {0}";

            switch (state)
            {
                case TwitchClientState.Connected:
                    {
                        LibraryDebug.Warning(error, "already connected");
                    }
                    break;
                case TwitchClientState.Connecting:
                    {
                        LibraryDebug.Warning(error, "already connecting");
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        LibraryDebug.Warning(error, "currently disconnecting");
                    }
                    break;
                case TwitchClientState.Disconnected:
                default:
                    {
                        result = true;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Disconnects from the IRC.
        /// </summary>
        public void Disconnect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Disconnection process starting...");

            if (!CanDisconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Disconnection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            state = TwitchClientState.Disconnecting;

            try
            {
                LibraryDebug.PrintLine(debug_prefix + "Disconnecting from socket...");

                socket.Disconnect(true);

                LibraryDebug.PrintLine(debug_prefix + "Disconnected from socket");

                CompleteSocketDisconnect(false);            
            }
            catch(Exception exception)
            {
                state = TwitchClientState.Connected;

                LibraryDebug.PrintLine(debug_prefix + "Failed to disconnect from socket",
                                       LibraryDebug.FormatAsColumns(nameof(exception), exception.Message));
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Disconnection process aborted");
                LibraryDebug.BlankLine();
            }
        }

        /// <summary>
        /// Asynchronously disconnects from the IRC.
        /// </summary>
        public void DisconnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Async disconnection process starting...");

            if (!CanDisconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Async disconnection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            state = TwitchClientState.Disconnecting;

            LibraryDebug.PrintLine(debug_prefix + "Asynchronously disconnecting from socket...");

            socket.BeginDisconnect(true, Callback_OnSocketDisconnect, null);
        }

        /// <summary>
        /// Fired when the socket disconnected from the IRC, successful or not.
        /// Handles completing the disconnection if successful or aborting the process if it failed.
        /// </summary>
        /// <param name="result">The async result of operation.</param>
        private void Callback_OnSocketDisconnect(IAsyncResult result)
        {
            //if it fails just disconnect synchronously as a fail safe
            if (result.IsCompleted)
            {
                LibraryDebug.PrintLine(debug_prefix + "Asynchronously disconnected from socket");

                CompleteSocketDisconnect(true);
            }
            else
            {
                LibraryDebug.PrintLine(debug_prefix + "Failed to asynchronously disconnect from socket");
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Disconnection process aborted");
                LibraryDebug.BlankLine();

                state = TwitchClientState.Connected;
            }
        }

        /// <summary>
        /// Finishes the disconnection process.
        /// End the reader stream thread and closes the stream and socket.
        /// </summary>
        /// <param name="async">
        /// Whether this is called form an async process.
        /// Only used when reconnecting.
        /// Determines whether to reconnect normally or asynchronously.
        /// </param>
        private void CompleteSocketDisconnect(bool async)
        {
            stream.Close();
            stream.Dispose();

            LibraryDebug.PrintLine(debug_prefix + "Waiting for " + nameof(reader_thread).Wrap("\"", "\"") + " to stop...");

            bool polling = true;

            do
            {
                switch (reader_thread.ThreadState)
                {
                    case ThreadState.Aborted:
                    case ThreadState.Stopped:
                    case ThreadState.Suspended:
                        {
                            polling = false;
                        }
                        break;

                }

                Thread.Sleep(25);
            }
            while (polling);

            LibraryDebug.PrintLine(debug_prefix + nameof(reader_thread).Wrap("\"", "\"") + " stopped");

            socket.Close();
            socket.Dispose();

            state = TwitchClientState.Disconnected;

            if (reconnecting)
            {
                if (async)
                {
                    LibraryDebug.Header(TimeStamp.TimeLong,
                                        debug_prefix + "Async disconnection process completed",
                                        nameof(OnDisconnected).Wrap("\"", "\"") + " not raised, currently reconnecting");
                    ConnectAsync();
                }
                else
                {
                    LibraryDebug.Header(TimeStamp.TimeLong,
                                        debug_prefix + "Disconnection process completed",
                                        nameof(OnDisconnected).Wrap("\"", "\"") + " not raised, currently reconnecting");
                    Connect();
                }
            }
            else
            {
                OnDisconnected.Raise(this, EventArgs.Empty);

                if (async)
                {
                    LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Async disconnection process completed");
                }
                else
                {
                    LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Disconnection process completed");
                }

                LibraryDebug.BlankLine();
            }
        }

        /// <summary>
        /// Checks to see if it is safe to disconnect from the Twitch IRC.
        /// </summary>
        private bool CanDisconnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot disconnect from " + host.Wrap("\"", "\"") + ", {0}";

            switch (state)
            {
                case TwitchClientState.Connecting:
                    {
                        LibraryDebug.Warning(error, "currently connecting");
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        LibraryDebug.Warning(error, "currently disconnecting");
                    }
                    break;
                case TwitchClientState.Disconnected:
                    {
                        LibraryDebug.Warning(error, "already disconnected");
                    }
                    break;
                case TwitchClientState.Connected:
                default:
                    {
                        result = true;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Reconnect to the IRC.
        /// </summary>
        public void Reconnect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Reconnection process starting...");

            if (!CanReconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Resconnection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            reconnecting = true;

            if(state == TwitchClientState.Connected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        /// <summary>
        /// Asynchronously reconnect to the IRC.
        /// </summary>
        public void ReconnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, debug_prefix + "Async reconnection process starting...");

            if (!CanReconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, debug_prefix + "Async resconnection process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            reconnecting = true;

            if (state == TwitchClientState.Connected)
            {
                DisconnectAsync();
            }
            else
            {
                ConnectAsync();
            }
        }

        /// <summary>
        /// Checks to see if it is safe to reconnect to the Twitch IRC.
        /// </summary>
        private bool CanReconnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot resconnect to " + host.Wrap("\"", "\"") + ", {0}";

            switch (state)
            {
                case TwitchClientState.Connecting:
                    {
                        LibraryDebug.Warning(error, "currently connecting");
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        LibraryDebug.Warning(error, "currently disconnecting");
                    }
                    break;
                case TwitchClientState.Connected:
                case TwitchClientState.Disconnected:
                default:
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        #endregion

        #region Threads

        /// <summary>
        /// Reads the data coming in from the IRC through the stream.
        /// </summary>
        private async void ReadStream()
        {
            while (!socket.isNull() && socket.Connected && !stream.isNull())
            {
                try
                {
                    byte[] buffer = new byte[1024 * 2];
                    int byte_count = await stream.ReadAsync(buffer, 0, buffer.Length);

                    LibraryDebug.Header(debug_prefix + "Data receievd from Irc. Processing starting...");

                    if (!buffer.isValid() || byte_count <= 0)
                    {
                        LibraryDebug.Warning(debug_prefix + "Data was null or empty");

                        continue;
                    }

                    do
                    {
                        // 0x0A = '\n'
                        int byte_index = Array.IndexOf<byte>(buffer, 0x0A, 0, buffer.Length);

                        // The irc spec specifies messages will always have a \r\n at the end.
                        // This is for when shit hits the fan and some funky shit happened
                        if (byte_index < 0)
                        {
                            LibraryDebug.Warning(debug_prefix + "Data was incomplete and did not contain \\n");
                            LibraryDebug.BlankLine();

                            break;
                        }

                        byte_index++;

                        string message = encoding.GetString(buffer, 0, byte_index - 2);
                        LibraryDebug.PrintLine(debug_prefix + "Data successfully converted from bytes into a string",
                                               LibraryDebug.FormatAsColumns(nameof(message), message));
                        ProcessIrcMessage(message);                        

                        Buffer.BlockCopy(buffer, byte_index, buffer, 0, byte_count - byte_index);
                        byte_count -= byte_index;
                    }
                    while (byte_count > 0);

                    LibraryDebug.Header(debug_prefix + "Irc data processing complete");
                    LibraryDebug.BlankLine();
                }
                catch (Exception exception)
                {
                    // only an error if the client is not disconnecting
                    if(state != TwitchClientState.Disconnecting)
                    {
                        LibraryDebug.Error(debug_prefix + "Error while reading from stream",
                                           nameof(exception), exception.Message);
                    }
                }
            }

            stream.Close();
            stream.Dispose();
        }

        #endregion

        #region Processors

        /// <summary>
        /// Received the convereted messages from the stream and processes them according to the IRC spec.
        /// </summary>
        /// <param name="raw_message">The string version of the data receievd through the stream.</param>
        private void ProcessIrcMessage(string raw_message)
        {
            // TODO: (IrcClient) Move messages that are not a PING to a process queue so any PING that comes in can be handled right away.

            IrcMessage irc_message = new IrcMessage(raw_message);
            LibraryDebug.Notify(debug_prefix + "Irc message receieved. Processing starting...");
            OnIrcMessage.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));

            switch (irc_message.command)
            {
                case "001":
                    {
                        state = TwitchClientState.Connected;

                        if (reconnecting)
                        {
                            reconnecting = false;
                            OnReconnected.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));

                            LibraryDebug.Header(debug_prefix + "Connection process completed",
                                                nameof(OnConnected) + " not called due to reconnection process1");
                            LibraryDebug.Header(debug_prefix + "Reconnection process completed");
                            LibraryDebug.BlankLine();
                        }
                        else
                        {
                            OnConnected.Raise(this, EventArgs.Empty);

                            LibraryDebug.Header(debug_prefix + "Connection process completed");
                            LibraryDebug.BlankLine();
                        }
                    }
                    break;
                case "PING":
                    {
                        if (auto_pong)
                        {
                            Ping(irc_message);
                        }

                        OnPing.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));
                    }
                    break;
                case "PRIVMSG":
                    {
                        // NOTE: (IrcClient) IRC Command - PRIVMSG tags only sent when /tags is requested
                        // TODO: (IrcClient) IRC Command - PRIVMSG sent_ts and tmi_sent_ts don't seem to be deserialized properly, fix this
                        OnPrivmsg.Raise(this, new PrivmsgEventArgs(raw_message, irc_message));
                    }
                    break;
                case "JOIN":
                    {
                        // NOTE: (IrcClient) IRC Command - JOIN requires /membership per the Twitch doc, however it has been confirmed to function the same even without requesting /membership 
                        OnJoin.Raise(this, new JoinEventArgs(raw_message, irc_message));
                    }
                    break;
                case "PART":
                    {
                        // NOTE: (IrcClient) IRC Command - PART requires /membership per the Twitch doc, however it has been confirmed to function the same even without requesting /membership 
                        OnPart.Raise(this, new PartEventArgs(raw_message, irc_message));
                    }
                    break;
                case "MODE":
                    {
                        // NOTE: (IrcClient) IRC Command - MODE requires /membership, this is extremely unreliable
                        OnMode.Raise(this, new ModeEventArgs(raw_message, irc_message));
                    }
                    break;
                case "353":
                    {
                        // :rokubotto.tmi.twitch.tv 353 rokubotto = #rokuhodo_ :rokubotto

                        // NOTE: (IrcClient) IRC Command - 353 requires /membership, will only list OP users is number of users in the room > 1000
                        string channel_name = irc_message.middle[2].TextAfter('#');
                        if (!names.ContainsKey(channel_name) || !names[channel_name].isValid())
                        {
                            names[channel_name] = new List<string>();
                        }

                        foreach (string name in irc_message.trailing)
                        {
                            names[channel_name].Add(name);
                        }
                    }
                    break;
                case "366":
                    {
                        // NOTE: (IrcClient) IRC Command - 366 (NAMES) requires /membership
                        string channel_name = irc_message.middle[1].TextAfter('#');
                        OnNames.Raise(this, new NamesEventArgs(raw_message, irc_message, names[channel_name]));
                        names[channel_name].Clear();
                    }
                    break;
                default:
                    {
                        LibraryDebug.PrintLine("Unsupported command recieved from Twitch IRC");
                        OnUnsupportedCommand.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));
                    }
                    break;
            }

            LibraryDebug.Notify(debug_prefix + "Irc message processing completed");
        }

        #endregion

        #region Sending data and commands              

        public void Send(string message, params object[] format)
        {
            LibraryDebug.Header(debug_prefix + "Send proccess starting...");

            message = string.Format(message, format);
            if (!CanSendMessage(message))
            {
                LibraryDebug.Error(debug_prefix + "Send process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            try
            {
                byte[] bytes = encoding.GetBytes(message + "\r\n");
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                LibraryDebug.PrintLine(debug_prefix + "Message sent to Irc",
                                       LibraryDebug.FormatAsColumns(nameof(message), message));

                OnMessageSent.Raise(this, new MessageEventArgs(message));
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(debug_prefix + "Send process aborted, compiler exception",
                                   LibraryDebug.FormatAsColumns(nameof(exception), exception.Message));
            }

            LibraryDebug.Header(debug_prefix + "Send proccess completed");
            LibraryDebug.BlankLine();
        }

        public async void SendAsync(string message, params object[] format)
        {
            LibraryDebug.Header(debug_prefix + "Asyn send proccess starting...");

            message = string.Format(message, format);
            if (!CanSendMessage(message))
            {
                LibraryDebug.Error(debug_prefix + "Async send process aborted");
                LibraryDebug.BlankLine();

                return;
            }

            try
            {
                byte[] bytes = encoding.GetBytes(message + "\r\n");
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Flush();

                LibraryDebug.PrintLine(debug_prefix + "Message asynchronously sent to Irc",
                                       LibraryDebug.FormatAsColumns(nameof(message), message));

                OnMessageSent.Raise(this, new MessageEventArgs(message));
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(debug_prefix + "Async send process aborthed, compiler exception",
                                   LibraryDebug.FormatAsColumns(nameof(exception), exception.Message));
            }

            LibraryDebug.Header(debug_prefix + "Async send proccess completed");
            LibraryDebug.BlankLine();
        }        

        public void SendPrivmsg(string channel, string message)
        {
            Send("PRIVMSG #{0}: {1}", channel.ToLower(), message);
        }

        public void SendPrivmsg(string channel, params object[] parts)
        {
            string message = string.Join(" ", parts);
            SendPrivmsg(channel.ToLower(), message);
        }

        public void SendPrivmsg(string channel, string message, params object[] format)
        {
            message = string.Join(message, format);
            SendPrivmsg(channel.ToLower(), message);
        }

        public void Ping()
        {
            Send("PING");
        }

        public void Ping(string parmaters)
        {
            Send("PING: {0}", parmaters);
        }

        public void Ping(IrcMessage irc_message)
        {
            Ping(irc_message.parameters);
        }

        /// <summary>
        /// Join a series of channel rooms.
        /// </summary>        
        public void Join(params string[] channels)
        {
            string channel_names = string.Join(",#", channels).ToLower();
            LibraryDebug.Notify("Joining room(s): #" + channel_names);
            Send("JOIN #" + channel_names);
        }

        /// <summary>
        /// Join a list of channel rooms.
        /// </summary>    
        public void Join(List<string> channels)
        {
            string channel_names = string.Join(",#", channels).ToLower();
            Join(channel_names);
        }

        /// <summary>
        /// Leave a series of channel rooms.
        /// </summary>        
        public void Part(params string[] channels)
        {
            string channel_names = string.Join(",#", channels).ToLower();
            LibraryDebug.Notify("Leaving room(s): #" + channel_names);
            Send("PART #" + channel_names.ToLower());
        }

        /// <summary>
        /// Leave a list of channel rooms.
        /// </summary>
        public void Part(List<string> _channel_names)
        {
            string channel_names = string.Join(",#", _channel_names).ToLower();
            Part(channel_names);
        }

        private bool CanSendMessage(string message)
        {
            bool result = true;

            string error = debug_prefix + "Cannot send message to Irc, {0}";

            if (!socket.Connected || socket.isNull())
            {
                LibraryDebug.Warning(error, "socket is not connected or has been closed");

                result = false;
            }
            else if (stream.isNull())
            {
                LibraryDebug.Warning(error, "stream is closed or null");

                result = false;
            }
            else if (!message.isValid())
            {
                LibraryDebug.Warning(error, "message is empty or null");

                result = false;
            }
            else if (message.Length > 512)
            {
                LibraryDebug.Warning(error, "message is greater than 512 characters");

                result = false;
            }

            return result;
        }

        #endregion
    }
}