// standard namespaces
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

// project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Clients.IRC;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients.IRC;
using TwitchLibrary.Events.Clients.IRC.Commands.Native;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.Clients.IRC;

namespace TwitchLibrary.Clients.IRC
{
    public class IrcClient
    {
        #region Fields

        // Private

        private bool                                    reconnecting;

        private int                                     port;

        private string                                  host;        

        private Dictionary<string, List<string>>        names;

        private Encoding                                encoding;
        private Socket                                  socket;
        private NetworkStream                           stream;
        private Thread                                  reader_thread;

        private IrcUser                                 irc_user;

        private Mutex                                   state_mutex;

        // Protected

        protected string                                debug_prefix;

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
        /// Raised when an unknown command is sent to the IRC.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> OnUnknownCommand;

        #endregion

        #region Properties

        /// <summary>
        /// The current state of the client.
        /// </summary>
        public TwitchClientState state { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates an <see cref="IrcClient"/> <see cref="object"/>.
        /// </summary>
        /// <param name="host">The name of the remove host.</param>
        /// <param name="port">The port of the remote host.</param>
        /// <param name="irc_user">The user that will be logged into the IRC.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the passed arguments are null.</exception>
        /// <exception cref="ArgumentException">Thrown when the host or irc_user fileds are empty or white space.</exception>
        public IrcClient(string _host, int _port, IrcUser _irc_user)
        {
            if (_irc_user.isNull())
            {
                throw new ArgumentNullException(nameof(_irc_user));
            }

            if (_irc_user.nick.isNull())
            {
                throw new ArgumentNullException(nameof(_irc_user.nick));
            }

            if (!_irc_user.nick.isValid())
            {
                throw new ArgumentException(Error.EXCEPTION_ARGUMENT_EMPTY, nameof(_irc_user.nick));
            }

            if (_irc_user.pass.isNull())
            {
                throw new ArgumentNullException(nameof(_irc_user.pass));
            }            

            if (!_irc_user.pass.isValid())
            {
                throw new ArgumentException(Error.EXCEPTION_ARGUMENT_EMPTY, nameof(_irc_user.nick));
            }

            state_mutex     = new Mutex();
            SetState(TwitchClientState.Disconnected);

            auto_pong       = true;
            reconnecting    = false;

            port            = _port;
            host            = _host;

            irc_user        = _irc_user;
            debug_prefix    = "IrcClient " + irc_user.nick.Wrap("\"", "\"") + " - ";

            encoding        = Encoding.UTF8;

            names           = new Dictionary<string, List<string>>();
        }

        #endregion

        #region Connection methods        

        /// <summary>
        /// Connects to the IRC.
        /// </summary>
        public void Connect()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Connection process starting...");

            if (!SetState(TwitchClientState.Connecting))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Connection process aborted");
                Log.BlankLine();

                return;
            }

            try
            {
                Log.PrintLine(debug_prefix + "Connecting to socket...",
                              debug_prefix + Log.FormatColumns(nameof(host), host),
                              debug_prefix + Log.FormatColumns(nameof(port), port.ToString()));

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);

                Log.PrintLine(debug_prefix + "Connected to socket");

                CompleteSocketConnect();
            }
            catch(Exception exception)
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to connecting to socket",
                                              debug_prefix + Error.NORMAL_EXCEPTION,
                                              debug_prefix + Log.FormatColumns(nameof(exception), exception.Message),
                                              debug_prefix + "Connection process aborted");
                Log.BlankLine();

                socket.Close();
                socket.Dispose();

                SetState(TwitchClientState.Disconnected);
            }
        }

        /// <summary>
        /// Asynchronously connects to the IRC.
        /// </summary>
        public void ConnectAsync()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Async connection process starting...");

            if (!SetState(TwitchClientState.Connecting))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Async connection process aborted");
                Log.BlankLine();

                return;
            }

            Log.PrintLine(debug_prefix + "Asynchronously connecting to socket...",
                          debug_prefix + Log.FormatColumns(nameof(host), host),
                          debug_prefix + Log.FormatColumns(nameof(port), port.ToString()));

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
                Log.PrintLine(debug_prefix + "Asynchronously connected to socket");

                CompleteSocketConnect();
            }
            else
            {
                stream.Close();
                stream.Dispose();

                socket.Close();
                socket.Dispose();

                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to asynchronously connect to socket",
                                              debug_prefix + "Async connection process aborted");
                Log.BlankLine();

                SetState(TwitchClientState.Disconnected);
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

            Log.PrintLine(debug_prefix + "Signing into IRC " + host.Wrap("\"", "\""));

            Send("PASS oauth:" + irc_user.pass);
            Send("NICK " + irc_user.nick);
        }                

        /// <summary>
        /// Disconnects from the IRC.
        /// </summary>
        public void Disconnect()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Disconnection process starting...");

            if (!SetState(TwitchClientState.Disconnecting))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Disconnection process aborted");
                Log.BlankLine();

                return;
            }

            try
            {
                Log.PrintLine(debug_prefix + "Disconnecting from socket...");

                socket.Disconnect(true);

                Log.PrintLine(debug_prefix + "Disconnected from socket");

                CompleteSocketDisconnect(false);            
            }
            catch(Exception exception)
            {
                SetState(TwitchClientState.Connected);

                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to disconnect from socket",
                                              debug_prefix + Error.NORMAL_EXCEPTION,
                                              debug_prefix + Log.FormatColumns(nameof(exception), exception.Message),
                                              debug_prefix + "Disconnection process aborted");
                Log.BlankLine();
            }
        }

        /// <summary>
        /// Asynchronously disconnects from the IRC.
        /// </summary>
        public void DisconnectAsync()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Async disconnection process starting...");

            if (!SetState(TwitchClientState.Disconnecting))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Async disconnection process aborted");
                Log.BlankLine();

                return;
            }

            Log.PrintLine(debug_prefix + "Asynchronously disconnecting from socket...");

            socket.BeginDisconnect(true, Callback_OnSocketDisconnect, null);
        }

        /// <summary>
        /// Fired when the socket disconnected from the IRC, successful or not.
        /// Handles completing the disconnection if successful or aborting the process if it failed.
        /// </summary>
        /// <param name="result">The async result of operation.</param>
        private void Callback_OnSocketDisconnect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                Log.PrintLine(debug_prefix + "Asynchronously disconnected from socket");

                CompleteSocketDisconnect(true);
            }
            else
            {                
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to asynchronously disconnect from socket",
                                              debug_prefix + "Disconnection process aborted");
                Log.BlankLine();

                SetState(TwitchClientState.Connected);
            }
        }

        /// <summary>
        /// Finishes the disconnection process.
        /// End the reader stream thread and closes the stream and socket.
        /// </summary>
        /// <param name="async">
        /// Whether this is called from an async process.
        /// Determines whether to reconnect normally or asynchronously.
        /// </param>
        private void CompleteSocketDisconnect(bool async)
        {
            stream.Close();
            stream.Dispose();

            Log.PrintLine(debug_prefix + "Waiting for " + nameof(reader_thread).Wrap("\"", "\"") + " to stop...");

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

            socket.Close();
            socket.Dispose();

            SetState(TwitchClientState.Disconnected);

            if (reconnecting)
            {
                if (async)
                {
                    Log.Success(debug_prefix + "Successfully asynchronously disconnected from " + host.Wrap("\"", "\""),
                                debug_prefix + nameof(OnDisconnected).Wrap("\"", "\"") + " not raised, currently reconnecting");

                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Async disconnection process completed");
                                        
                    ConnectAsync();
                }
                else
                {
                    Log.Success(debug_prefix + "Successfully disconnected from " + host.Wrap("\"", "\""),
                                debug_prefix + nameof(OnDisconnected).Wrap("\"", "\"") + " not raised, currently reconnecting");

                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Disconnection process completed");
                    Connect();
                }
            }
            else
            {               
                OnDisconnected.Raise(this, EventArgs.Empty);

                if (async)
                {
                    Log.Success(debug_prefix + "Successfully asynchronously disconnected from " + host.Wrap("\"", "\""));
                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Async disconnection process completed");
                }
                else
                {
                    Log.Success(debug_prefix + "Successfully disconnected from " + host.Wrap("\"", "\""));
                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Disconnection process completed");
                }

                Log.BlankLine();
            }
        }        

        /// <summary>
        /// Reconnect to the IRC.
        /// </summary>
        public void Reconnect()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Reconnection process starting...");

            if (!SetState(state, true))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Resconnection process aborted");
                Log.BlankLine();

                return;
            }

            reconnecting = true;

            state_mutex.WaitOne();
            if (state == TwitchClientState.Connected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
            state_mutex.ReleaseMutex();
        }

        /// <summary>
        /// Asynchronously reconnect to the IRC.
        /// </summary>
        public void ReconnectAsync()
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Async reconnection process starting...");

            if (!CanReconnect())
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Async resconnection process aborted");
                Log.BlankLine();

                return;
            }

            reconnecting = true;

            state_mutex.WaitOne();
            if (state == TwitchClientState.Connected)
            {
                DisconnectAsync();
            }
            else
            {
                ConnectAsync();
            }
            state_mutex.ReleaseMutex();
        }

        #endregion

        #region State changes

        /// <summary>
        /// Safely changes the state of the client.
        /// </summary>
        /// <param name="transition_state">
        /// The state to change the current state to.
        /// This transition state is meaningless when attempting a reconnect becasue the state is actually changed by Connect() or Disconnect().
        /// </param>
        /// <param name="attempting_reconnect">Flag that determines if the client is trying to reconnect.</param>
        /// <returns></returns>
        private bool SetState(TwitchClientState transition_state, bool attempting_reconnect = false)
        {
            bool change_state = false;

            state_mutex.WaitOne();
            switch (transition_state)
            {
                case TwitchClientState.Connecting:
                    {
                        change_state = attempting_reconnect ? CanReconnect() : CanConnect();
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        change_state = attempting_reconnect ? CanReconnect() : CanDisconnect();
                    }
                    break;
                case TwitchClientState.Connected:
                case TwitchClientState.Disconnected:
                    {
                        change_state = attempting_reconnect ? CanReconnect() : true;
                    }
                    break;
            }

            if (change_state)
            {
                state = transition_state;
            }
            state_mutex.ReleaseMutex();

            return change_state;
        }

        /// <summary>
        /// Checks to see if it is safe to connect to the IRC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanConnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot connect to " + host.Wrap("\"", "\"") + ", {0}";

            switch (state)
            {
                case TwitchClientState.Connected:
                    {
                        Log.Warning(string.Format(error, "already connected"));
                    }
                    break;
                case TwitchClientState.Connecting:
                    {
                        Log.Warning(string.Format(error, "already connecting"));
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        Log.Warning(string.Format(error, "currently disconnecting"));
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
        /// Checks to see if it is safe to disconnect from the Twitch IRC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanDisconnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot disconnect from " + host.Wrap("\"", "\"") + ", {0}";

            switch (state)
            {
                case TwitchClientState.Connecting:
                    {
                        Log.Warning(string.Format(error, "currently connecting"));
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        Log.Warning(string.Format(error, "currently disconnecting"));
                    }
                    break;
                case TwitchClientState.Disconnected:
                    {
                        Log.Warning(string.Format(error, "already disconnected"));
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
        /// Checks to see if it is safe to reconnect to the Twitch IRC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanReconnect()
        {
            bool result = false;

            string error = debug_prefix + "Cannot resconnect to " + host.Wrap("\"", "\"") + ", {0}";

            switch (state)
            {
                case TwitchClientState.Connecting:
                    {
                        Log.Warning(string.Format(error, "currently connecting"));
                    }
                    break;
                case TwitchClientState.Disconnecting:
                    {
                        Log.Warning(string.Format(error, "currently disconnecting"));
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

                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Data receievd from Irc. Processing starting...");

                    if (!buffer.isValid() || byte_count <= 0)
                    {
                        Log.Warning(debug_prefix + "Data was null or empty");

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
                            Log.Warning(debug_prefix + "Data was incomplete and did not contain \\n");
                            Log.BlankLine();

                            break;
                        }

                        byte_index++;

                        string message = encoding.GetString(buffer, 0, byte_index - 2);
                        Log.PrintLine(debug_prefix + "Data successfully converted from bytes into a string",
                                      debug_prefix + Log.FormatColumns(nameof(message), message));
                        ProcessIrcMessage(message);                        

                        Buffer.BlockCopy(buffer, byte_index, buffer, 0, byte_count - byte_index);
                        byte_count -= byte_index;
                    }
                    while (byte_count > 0);

                    Log.Header(TimeStamp.TimeLong, debug_prefix + "Irc data processing complete");
                }
                catch (Exception exception)
                {
                    // only an error if the client is not disconnecting
                    if(state != TwitchClientState.Disconnecting)
                    {
                        Log.Error(TimeStamp.TimeLong, debug_prefix + "Error while reading from stream, messages skipped",
                                                      debug_prefix + Error.NORMAL_EXCEPTION,
                                                      debug_prefix + nameof(exception), exception.Message);
                    }
                }
                
                Log.BlankLine();
            }

            Log.Warning(debug_prefix + nameof(reader_thread).Wrap("\"", "\"") + " stopped");

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

            Log.PrintLine(debug_prefix + "Irc message receieved. Processing starting...");

            IrcMessage irc_message = new IrcMessage(raw_message);
            OnIrcMessage.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));

            switch (irc_message.command)
            {
                case "001":
                    {
                        SetState(TwitchClientState.Connected);

                        Log.Success(debug_prefix + "Successfully connected to " + host.Wrap("\"", "\""));                        

                        if (reconnecting)
                        {
                            Log.Success(debug_prefix + "Successfully reconnected to " + host.Wrap("\"", "\""),
                                        debug_prefix + nameof(OnConnected) + " not called due to reconnection process");

                            reconnecting = false;
                            OnReconnected.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));

                            Log.Header(debug_prefix + "Reconnection process completed");
                            Log.Header(debug_prefix + "Connection process completed");
                        }
                        else
                        {
                            OnConnected.Raise(this, EventArgs.Empty);

                            Log.Header(debug_prefix + "Connection process completed");
                        }

                        Log.BlankLine();
                    }
                    break;
                case "PING":
                    {
                        if (auto_pong)
                        {
                            Log.Header(debug_prefix + "Sending auto pong...");
                            Pong(irc_message);
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

                #region Unknown command

                case "421":
                    {
                        Log.PrintLine("Unkown command recieved from Twitch IRC");
                        OnUnknownCommand.Raise(this, new IrcMessageEventArgs(raw_message, irc_message));
                    }
                    break;

                #endregion
            }

            Log.PrintLine(debug_prefix + "Irc message processing completed");
        }

        #endregion

        #region Sending data and commands                      

        /// <summary>
        /// Sends a PRIVMSG to the IRC.
        /// </summary>
        /// <param name="channel">The channel/room to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="format">An object array that contains zero or more object to format.</param>
        public bool SendPrivmsg(string channel, string message, params object[] format)
        {
            bool success = false;

            message = string.Format(message, format);
            success = Send("PRIVMSG #{0} :{1}", channel.ToLower(), message);

            return success;
        }

        /// <summary>
        /// Sends a PONG message to the IRC.
        /// </summary>
        public bool Pong()
        {
            return Send("PONG");
        }

        /// <summary>
        /// Sends a PONG message to the IRC.
        /// </summary>
        /// <param name="parmaters">Parameteres that would normally be found in an <see cref="IrcMessage"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Pong(string parmaters)
        {
            return Send("PONG: {0}", parmaters);
        }

        /// <summary>
        /// Sends a PONG message to the IRC.
        /// </summary>
        /// <param name="irc_message">The <see cref="IrcMessage"/> that contains the parameters to be sent with the PONG.</param>
        public bool Pong(IrcMessage irc_message)
        {
            return Pong(irc_message.parameters);
        }

        /// <summary>
        /// Join a series of channel rooms.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Join(params string[] channels)
        {
            bool success = false;

            string channel_names = string.Join(",#", channels).ToLower();
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Joining room(s): #" + channel_names);
            success = Send("JOIN #" + channel_names);

            return success;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Part(params string[] channels)
        {
            bool success = false;

            string channel_names = string.Join(",#", channels).ToLower();
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Leaving room(s): #" + channel_names);
            success = Send("PART #" + channel_names.ToLower());

            return success;
        }

        /// <summary>
        /// Leave a list of channel rooms.
        /// </summary>
        public void Part(List<string> _channel_names)
        {
            string channel_names = string.Join(",#", _channel_names).ToLower();
            Part(channel_names);
        }

        /// <summary>
        /// Sends a message to the IRC.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="format">An object array that contains zero or more object to format.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool Send(string message, params object[] format)
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Send proccess starting...");

            bool success = false;

            message = string.Format(message, format).RemovePadding();
            if (!CanSendMessage(message))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Send process aborted");
                Log.BlankLine();

                return success;
            }

            try
            {
                byte[] bytes = encoding.GetBytes(message + "\r\n");
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                success = true;

                Log.PrintLine(debug_prefix + "Message sent to Irc",
                              debug_prefix + Log.FormatColumns(nameof(message), message));

                OnMessageSent.Raise(this, new MessageEventArgs(message));

                Log.Header(debug_prefix + "Send proccess completed");
            }
            catch (Exception exception)
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to send message",
                                              debug_prefix + Error.NORMAL_EXCEPTION,
                                              debug_prefix + Log.FormatColumns(nameof(exception), exception.Message),
                                              debug_prefix + "Send process aborted");
            }

            Log.BlankLine();

            return success;
        }

        /// <summary>
        /// Asynchronously sends a message to the IRC.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="format">An object array that contains zero or more object to format.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected async Task<bool> SendAsync(string message, params object[] format)
        {
            Log.Header(TimeStamp.TimeLong, debug_prefix + "Asyn send proccess starting...");

            bool success = false;

            message = string.Format(message, format).RemovePadding();
            if (!CanSendMessage(message))
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Async send process aborted");
                Log.BlankLine();

                return success;
            }

            try
            {
                byte[] bytes = encoding.GetBytes(message + "\r\n");
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Flush();

                success = true;

                Log.PrintLine(debug_prefix + "Message asynchronously sent to Irc",
                              debug_prefix + Log.FormatColumns(nameof(message), message));

                OnMessageSent.Raise(this, new MessageEventArgs(message));

                Log.Header(TimeStamp.TimeLong, debug_prefix + "Async send proccess completed");
            }
            catch (Exception exception)
            {
                Log.Error(TimeStamp.TimeLong, debug_prefix + "Failed to asynchronously send message",
                                              debug_prefix + Error.NORMAL_EXCEPTION,
                                              debug_prefix + Log.FormatColumns(nameof(exception), exception.Message),
                                              debug_prefix + "Async send process aborted");
            }

            Log.BlankLine();

            return success;
        }

        /// <summary>
        /// Performs checks and determines whether or not the message can be sent.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CanSendMessage(string message)
        {
            bool result = true;

            string error = debug_prefix + "Cannot send message to Irc, {0}";

            if (!socket.Connected || socket.isNull())
            {
                Log.Warning(string.Format(error, "socket is not connected or has been closed"));

                result = false;
            }
            else if (stream.isNull())
            {
                Log.Warning(string.Format(error, "stream is closed or null"));

                result = false;
            }
            else if (!message.isValid())
            {
                Log.Warning(string.Format(error, "message is empty or null"));

                result = false;
            }
            else if (message.Length > 512)
            {
                Log.Warning(string.Format(error, "message is greater than 512 characters"));

                result = false;
            }

            return result;
        }

        #endregion
    }
}