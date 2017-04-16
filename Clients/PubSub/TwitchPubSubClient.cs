//standard namespaces
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Clients.PubSub;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients.PubSub;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits;
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Request;
using TwitchLibrary.Models.Clients.PubSub.Response;

//imported .dll's
using Newtonsoft.Json;
using WebSocketSharp;

//TODO: (PubSub) Create an ID system to uniquely identify each instance in case the user has multiple connections at once

namespace TwitchLibrary.Clients.PubSub
{
    public class TwitchPubSubClient
    {
        //private
        private bool reconnecting = false;
        private bool pending_pong = false;

        //TODO: (PubSub) This will overflow in the most extreme of cases, but substitute the nonce_id with somthing that will be safer
        private ulong nonce_id = 0;

        private readonly string WEB_SOCKET = "wss://pubsub-edge.twitch.tv";
        private string oauth_token;

        private DateTime ping_sent_time;

        private WebSocket web_socket;

        private Thread PubSubTimer_Thread;

        //public
        public bool auto_reconnect = true;

        public event EventHandler<EventArgs> OnConnect;
        public event EventHandler<EventArgs> OnReconnect;
        public event EventHandler<EventArgs> OnDisconnect;
        public event EventHandler<MessageTypeEventArgs> OnRecconectReceived;

        public event EventHandler<ErrorReceivedEventArgs> OnErrorReceived;

        public event EventHandler<EventArgs> OnPingSent;
        public event EventHandler<MessageTypeEventArgs> OnPongReceived;
        
        public event EventHandler<BitsReceivedEventArgs> OnBitsReceived;
        public event EventHandler<WhisperReceivedEventArgs> OnWhisperReceived;
        public event EventHandler<ResponseReceivedEventArgs> OnResponseReceived;
        public event EventHandler<UnsupportedMessageTypeReceivedEventArgs> OnUnsupportedMessageTypeReceived;

        public TwitchPubSubClient(string _oauth_token)
        {
            oauth_token = _oauth_token;
            InitSocket();
        }

        /// <summary>
        /// Initiates a new instance of the web socket for connects and reconnects.
        /// </summary>
        private void InitSocket()
        {
            web_socket = new WebSocket(WEB_SOCKET);
            web_socket.OnOpen += new EventHandler(OnOpen);
            web_socket.OnClose += new EventHandler<CloseEventArgs>(OnClose);
            web_socket.OnMessage += new EventHandler<MessageEventArgs>(OnMessage);
            web_socket.OnError += new EventHandler<ErrorEventArgs>(OnError);
        }

        #region Connection handling

        /// <summary>
        /// Asynchronously connects to the web socket.
        /// </summary>
        public void ConnectAsync()
        {
            if (!CanConnect())
            {
                return;
            }

            ping_sent_time = DateTime.Now;

            LibraryDebug.Notify("Asynchronously connecting  to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            web_socket.ConnectAsync();
        }

        /// <summary>
        /// Connects to the web socket.
        /// </summary>
        public void Connect()
        {
            if (!CanConnect())
            {
                return;
            }

            ping_sent_time = DateTime.Now;

            LibraryDebug.Notify("Connecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            web_socket.Connect();
        }

        /// <summary>
        /// Checks to see if it is safe to connect to the web socket.
        /// </summary>
        private bool CanConnect()
        {
            //can't do WebSocketState.Connecting because it's the default state for some reason
            if (web_socket.ReadyState == WebSocketState.Open)
            {
                LibraryDebug.Warning("Cannot connect to PubSub socket, already connected to " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Asynchronously disconnects from the web socket.
        /// </summary>
        public void DisconnectAsync()
        {
            if (!CanDisconnect())
            {
                return;
            }

            LibraryDebug.Notify("Asynchronously disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            web_socket.CloseAsync();
        }

        /// <summary>
        /// Disconnects from the web socket.
        /// </summary>
        public void Disconnect()
        {
            if (!CanDisconnect())
            {
                return;
            }

            LibraryDebug.Notify("Disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            web_socket.Close();
        }

        /// <summary>
        /// Checks to see if it is safe to disconnect from the web socket.
        /// </summary>
        private bool CanDisconnect()
        {
            if (web_socket.ReadyState == WebSocketState.Closed)
            {
                LibraryDebug.Warning("Cannot discconect from PubSub socket, already disconnected from " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
                return false;
            }

            if (web_socket.ReadyState == WebSocketState.Closing)
            {
                LibraryDebug.Warning("Cannot discconect from PubSub socket, already disconnecting from " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Asynchronously reconnects to the web socket.
        /// </summary>
        public void ReconnectAsync()
        {
            if (!CanReconnect())
            {
                return;
            }

            LibraryDebug.Notify("Reconnecting asynchronously to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            reconnecting = true;
            web_socket.CloseAsync();
        }

        /// <summary>
        /// Reconnects to the web socket.
        /// </summary>
        public void Reconnect()
        {
            if (!CanReconnect())
            {
                return;
            }

            LibraryDebug.Notify("Reconnecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...", TimeStamp.TimeLong);
            reconnecting = true;
            web_socket.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CanReconnect()
        {
            if (web_socket.ReadyState == WebSocketState.Closing)
            {
                LibraryDebug.Warning("Cannot recconect to PubSub socket, currently disconnecting from " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);

                return false;
            }

            if (web_socket.ReadyState == WebSocketState.Connecting)
            {
                LibraryDebug.Warning("Cannot reconnect to PubSub socket, currently connecting to " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);

                return false;
            }

            return true;
        }

        #endregion

        #region Events

        private void OnOpen(object sender, EventArgs e)
        {
            LibraryDebug.Success("Connected to PubSub socket " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
            pending_pong = false;

            ListenAsync("whispers.45947671");

            PubSubTimer_Thread = new Thread(new ThreadStart(PubSubTimer));
            PubSubTimer_Thread.Start();

            OnConnect.RaiseAsync(this, new EventArgs());

            if (reconnecting)
            {
                LibraryDebug.Success("Reconnected to PubSub socket " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
                reconnecting = false;

                OnReconnect.RaiseAsync(this, new EventArgs());
            }            
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            LibraryDebug.Notify("Discconnected from PubSub socket " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);

            if (reconnecting)
            {
                ConnectAsync();
            }

            OnDisconnect.RaiseAsync(this, new EventArgs());
        }

        private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            LibraryDebug.Error("Error received from PubSub socket " + WEB_SOCKET.Wrap("\"", "\""), TimeStamp.TimeLong);
            LibraryDebug.PrintLine(nameof(e.Message), e.Message);

            OnErrorReceived.RaiseAsync(this, new ErrorReceivedEventArgs
            {
                exception = e.Exception,
                message = e.Message
            });
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            LibraryDebug.Notify("Data recieved from PubSub", TimeStamp.TimeLong);

            /*
            //NOTE: (PubSub) Bits event cannot be tested first hand, so this fake object has been created to mimic a response assuming the documentation is correct
            PubSubMessage fake_bits = new PubSubMessage
            {
                type = "MESSAGE",
                data = new PubSubMessageData
                {
                    topic = "channel-bits-events-v1.44322889",
                    message = "{\"data\":{\"user_name\":\"dallasnchains\",\"channel_name\":\"dallas\",\"user_id\":\"129454141\",\"channel_id\":\"44322889\",\"chat_message\":\"cheer10000 New badge hype!\",\"bits_used\":10000,\"total_bits_used\":25000,\"context\":\"cheer\",\"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}},\"version\":\"1.0\",\"message_type\":\"bits_event\",\"message_id\":\"8145728a4-35f0-4cf7-9dc0-f2ef24de1eb6\"}"
                }
            };

            string fake_bits_message = JsonConvert.SerializeObject(fake_bits);
            */

            PubSubMessageType _type = await e.Data.TryDeserializeObjectAsync<PubSubMessageType>();
            Enum.TryParse(_type.type, true, out PubSubType type);
            switch (type)
            {
                case PubSubType.PONG:
                    {
                        LibraryDebug.PrintLine("PONG recieved from PubSub", TimeStamp.TimeLong);
                        pending_pong = false;

                        
                        OnPongReceived.RaiseAsync(this, new MessageTypeEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
                case PubSubType.RECONNECT:
                    {
                        LibraryDebug.PrintLine("RECONNECT recieved from PubSub", TimeStamp.TimeLong);

                        if (auto_reconnect)
                        {
                            ReconnectAsync();
                        }

                        OnRecconectReceived.RaiseAsync(this, new MessageTypeEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
                case PubSubType.RESPONSE:
                    {
                        LibraryDebug.PrintLine("RESPONSE recieved from PubSub", TimeStamp.TimeLong);
                        LibraryDebug.PrintObject(e.Data);

                        PubSubResponse response = await e.Data.TryDeserializeObjectAsync<PubSubResponse>();
                        OnResponseReceived.RaiseAsync(this, new ResponseReceivedEventArgs
                        {
                            data = e.Data,
                            response = response
                        });
                    }
                    break;
                case PubSubType.MESSAGE:
                    {
                        LibraryDebug.PrintLine("MESSAGE recieved from PubSub", TimeStamp.TimeLong);

                        PubSubMessage message = await e.Data.TryDeserializeObjectAsync<PubSubMessage>();
                        string topic = message.data.topic.TextBefore(".");

                        switch (topic)
                        {
                            case "whispers":
                                {
                                    LibraryDebug.PrintLine("Whisper message recieved from PubSub", TimeStamp.TimeLong);

                                    PubSubWhisperMessage whisper_message = JsonConvert.DeserializeObject<PubSubWhisperMessage>(message.data.message);
                                    PubSubWhisperMessageData whisper_message_data = JsonConvert.DeserializeObject<PubSubWhisperMessageData>(whisper_message.data);

                                    OnWhisperReceived.RaiseAsync(this, new WhisperReceivedEventArgs
                                    {
                                        data = e.Data,
                                        whisper_message = whisper_message,
                                        whisper_message_data = whisper_message_data
                                    });
                                }
                                break;
                            case "channel-bits-events-v1":
                                {
                                    LibraryDebug.PrintLine("Bits message recieved from PubSub", TimeStamp.TimeLong);
                                    PubSubBitsMessage bits_message = JsonConvert.DeserializeObject<PubSubBitsMessage>(message.data.message);

                                    OnBitsReceived.RaiseAsync(this, new BitsReceivedEventArgs
                                    {
                                        data = e.Data,
                                        bits_message = bits_message
                                    });
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        LibraryDebug.Error("Unsuported PubSub type recieved", TimeStamp.TimeLong);
                        LibraryDebug.PrintLine(nameof(type), type.ToString());

                        OnUnsupportedMessageTypeReceived.RaiseAsync(this, new UnsupportedMessageTypeReceivedEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
            }
        }

        #endregion

        #region Listen and Unlisten

        //TODO: (PubSub) Implement user-friendly way to listen to topics do they don't have to pass the raw strings
        public void Listen(string topic)
        {
            Listen(new List<string>() { topic });
        }

        public void Listen(List<string> topics)
        {
            SendTopicRequest(PubSubType.LISTEN, topics);
        }

        public void ListenAsync(string topic)
        {
            ListenAsync(new List<string>() { topic });
        }

        public void ListenAsync(List<string> topics)
        {
            SendTopicRequestAsync(PubSubType.LISTEN, topics);
        }

        //TODO: (PubSub) Implement user-friendly way to unlisten to topics do they don't have to pass the raw strings
        public void Unlisten(string topic)
        {
            Unlisten(new List<string>() { topic });
        }

        public void Unlisten(List<string> topics)
        {
            SendTopicRequest(PubSubType.UNLISTEN, topics);
        }

        public void UnlistenAsync(string topic)
        {
            UnlistenAsync(new List<string>() { topic });
        }

        public void UnlistenAsync(List<string> topics)
        {
            SendTopicRequestAsync(PubSubType.UNLISTEN, topics);
        }

        private void SendTopicRequest(PubSubType type, List<string> topics)
        {
            string type_string = type.ToString();

            PubSubRequest request = new PubSubRequest
            {
                type = type_string,
                nonce = type_string + nonce_id++,
                data = new PubSubRequestData
                {
                    topics = topics,
                    auth_token = oauth_token
                }
            };

            string request_serialized = JsonConvert.SerializeObject(request);

            web_socket.Send(request_serialized);
        }

        private async void SendTopicRequestAsync(PubSubType type, List<string> topics)
        {
            string type_string = type.ToString();

            PubSubRequest request = new PubSubRequest
            {
                type = type_string,
                nonce = type_string + nonce_id++,
                data = new PubSubRequestData
                {
                    topics = topics,
                    auth_token = oauth_token
                }
            };

            string request_serialized = "";

            try
            {
                request_serialized = await request.TrySerializeObjectAsync();
            }
            catch(TaskCanceledException exception)
            {
                LibraryDebug.PrintLine(exception.Message);
            }

            if (!request_serialized.isValid())
            {
                return;
            }

            web_socket.SendAsync(request_serialized, null);
        }

        #endregion

        #region Timer loop

        private void PubSubTimer()
        {
            LibraryDebug.Notify("PubSub timers started", TimeStamp.TimeLong);

            short pending_pong_limit_ms = 10 * 1000;
            int ping_frequency_ms = 4 * 60 * 1000;

            while (web_socket.ReadyState == WebSocketState.Open)
            {
                if(DateTime.Now - ping_sent_time > TimeSpan.FromMilliseconds(ping_frequency_ms))
                {
                    //native Ping() doesn't operate as requested by Twitch, send the raw string ourselves
                    web_socket.Send("{\"type\": \"PING\"}");
                    ping_sent_time = DateTime.Now;
                    pending_pong = true;

                    LibraryDebug.Notify("PING sent to PubSub", TimeStamp.TimeLong);
                    OnPingSent.RaiseAsync(this, new EventArgs());
                }

                if (pending_pong && DateTime.Now - ping_sent_time > TimeSpan.FromMilliseconds(pending_pong_limit_ms))
                {
                    LibraryDebug.Warning("PING sent to PubSub but did not receive a PONG after 10s. Issuing reconnect...", TimeStamp.TimeLong);
                    ReconnectAsync();
                }

                Thread.Sleep(50);
            }

            LibraryDebug.Warning("PubSub timers ended", TimeStamp.TimeLong);
        }

        #endregion
    }
}