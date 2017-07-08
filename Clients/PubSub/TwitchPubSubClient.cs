// standard namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

// project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Clients.PubSub;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Events.Clients;
using TwitchLibrary.Events.Clients.PubSub;
using TwitchLibrary.Extensions;
using TwitchLibrary.Extensions.Events;
using TwitchLibrary.Models.Clients.PubSub.Message;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Bits;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Subscriptions;
using TwitchLibrary.Models.Clients.PubSub.Message.Data.Whisper;
using TwitchLibrary.Models.Clients.PubSub.Request;
using TwitchLibrary.Models.Clients.PubSub.Response;

// imported .dll's
using Newtonsoft.Json;
using WebSocketSharp;

// TODO: (PubSub)

namespace TwitchLibrary.Clients.PubSub
{
    public class TwitchPubSubClient
    {
        #region Fields

        // private

        private bool                                                        reconnecting;

        private readonly int                                                PENDING_PONG_LIMIT_MS = 10 * 1000;
        private readonly int                                                PING_FREQUENCY_MS = 4 * 60 * 1000;

        private readonly string                                             WEB_SOCKET = "wss:// pubsub-edge.twitch.tv";
        private string                                                      id;
        private string                                                      oauth_token;

        private DateTime                                                    ping_sent_time;

        private Timer auto_ping_timer;
        private Timer pending_pong_timer;

        private WebSocket                                                   web_socket;

        // public

        /// <summary>
        /// Determines whether the client will reconnect automatically on disconnects.
        /// </summary>
        public bool                                                         auto_reconnect;

        /// <summary>
        /// Raised when the client successfully connected the socket.
        /// </summary>
        public event EventHandler<EventArgs>                                OnConnected;

        /// <summary>
        /// Raised when the client successfully reconnected to the socket.
        /// </summary>
        public event EventHandler<EventArgs>                                OnReconnected;

        /// <summary>
        /// Raised when the client successfully disconnects from the socket.
        /// </summary>
        public event EventHandler<MessageTypeEventArgs>                     OnRecconect;

        /// <summary>
        /// Raised when the client successfully reconnected to the socket.
        /// </summary>
        public event EventHandler<EventArgs>                                OnDisconnected;

        /// <summary>
        /// Raised when an error is received from the WebSocketSharp.dll.
        /// </summary>
        public event EventHandler<ErrorReceivedEventArgs>                   OnError;

        /// <summary>
        /// Raised when a PING was successfully sent to the socket either manually or automatically.
        /// </summary>
        public event EventHandler<EventArgs>                                OnPing;

        /// <summary>
        /// Raised when a PING failed to be sent to the socket either manually or automatically.
        /// </summary>
        public event EventHandler<EventArgs>                                OnPingFailed;

        /// <summary>
        /// Raised when a PONG has been recieved from the socket in response to a successful PING.
        /// </summary>
        public event EventHandler<MessageTypeEventArgs>                     OnPong;

        /// <summary>
        /// Raised when a user has cheered with bits.
        /// </summary>
        public event EventHandler<BitsReceivedEventArgs>                    OnBits;

        /// <summary>
        /// Raised when a MESSAGE has been received through the socket.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs>                 OnMessage;

        /// <summary>
        /// Raised when a user has subscribed to a channel.
        /// </summary>
        public event EventHandler<SubscriptionReceivedEventArgs>            OnSubscription;

        /// <summary>
        /// Raised when a whisper has been received by a user.
        /// </summary>
        public event EventHandler<WhisperReceivedEventArgs>                 OnWhisper;

        /// <summary>
        /// Raised when a RESPONSE has been receieved through the socket.
        /// </summary>
        public event EventHandler<ResponseReceivedEventArgs>                OnResponse;

        /// <summary>
        /// Raised when an unsupported MESSAGE type has been receieved through the socket.
        /// </summary>
        public event EventHandler<UnsupportedMessageTypeReceivedEventArgs>  OnUnsupportedMessageType;

        #endregion

        #region Constructors

        public TwitchPubSubClient(string _oauth_token, string _id = "")
        {
            auto_reconnect = true;
            reconnecting = false;

            id = _id;
            oauth_token = _oauth_token;                       

            web_socket = new WebSocket(WEB_SOCKET);
            web_socket.OnOpen += new EventHandler(OnWebSocketSharpOpen);
            web_socket.OnClose += new EventHandler<CloseEventArgs>(OnWebSocketSharpClose);
            web_socket.OnMessage += new EventHandler<MessageEventArgs>(OnWebSocketSharpMessage);
            web_socket.OnError += new EventHandler<ErrorEventArgs>(OnWebSocketSharpError);
        }

        #endregion

        #region Connection methods

        /// <summary>
        /// Asynchronously connects to the web socket.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        public void ConnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Async Connection Process Starting...");

            if (!CanConnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Async Connection Process Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            ping_sent_time = DateTime.Now;

            LibraryDebug.PrintLine("Asynchronously connecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.ConnectAsync();
        }

        /// <summary>
        /// Connects to the web socket.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        public void Connect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Connection Process Starting...");

            if (!CanConnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Connection Process Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            ping_sent_time = DateTime.Now;

            LibraryDebug.PrintLine("Connecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.Connect();
        }

        /// <summary>
        /// Checks to see if it is safe to connect to the web socket.
        /// </summary>
        private bool CanConnect()
        {
            bool result = true;

            // can't do WebSocketState.Connecting because it's the default state for some reason
            if (web_socket.ReadyState == WebSocketState.Open)
            {
                LibraryDebug.Warning("Cannot connect to PubSub socket, already connected.");
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Asynchronously disconnects from the web socket.
        /// </summary>
        public void DisconnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Async Disonnection Process Starting...");

            if (!CanDisconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Async Disconnection Process Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            LibraryDebug.PrintLine("Asynchronously disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.CloseAsync();
        }

        /// <summary>
        /// Disconnects from the web socket.
        /// </summary>
        public void Disconnect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Disonnection Process Starting...");

            if (!CanDisconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Disconnection Process Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            LibraryDebug.PrintLine("Disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.Close();
        }

        /// <summary>
        /// Checks to see if it is safe to disconnect from the web socket.
        /// </summary>
        private bool CanDisconnect()
        {
            bool result = true;

            if (web_socket.ReadyState == WebSocketState.Closed)
            {
                LibraryDebug.Warning("Cannot discconect from PubSub socket, already disconnected.");
                result = false;
            } 
            else if (web_socket.ReadyState == WebSocketState.Closing)
            {
                LibraryDebug.Warning("Cannot discconect from PubSub socket, already disconnecting.");
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Asynchronously reconnects to the web socket.
        /// After reconnecting, all topics must be LISTEN-ed to again manually.
        /// </summary>
        public void ReconnectAsync()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Async Reconnection Process Starting...");

            if (!CanReconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Async Reconnection Process Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            reconnecting = true;

            if (web_socket.ReadyState == WebSocketState.Open)
            {
                DisconnectAsync();
            }
            else
            {
                ConnectAsync();
            }
        }

        /// <summary>
        /// Reconnects to the web socket.
        /// After reconnecting, all topics must be LISTEN-ed to again manually.
        /// </summary>
        public void Reconnect()
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Reconnection Process Starting...");

            if (!CanReconnect())
            {
                LibraryDebug.Error(TimeStamp.TimeLong, "PubSub Reconnection Process Aborted.");
                LibraryDebug.BlankLine();
                return;
            }

            reconnecting = true;
            if(web_socket.ReadyState == WebSocketState.Open)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }            
        }

        /// <summary>
        /// Determines whether it is safe to reconnect.
        /// </summary>
        private bool CanReconnect()
        {
            bool result = true;

            if (web_socket.ReadyState == WebSocketState.Closing)
            {
                LibraryDebug.Warning("Cannot recconect to PubSub socket, currently disconnecting.");
                return false;
            }
            else if (web_socket.ReadyState == WebSocketState.Connecting)
            {
                LibraryDebug.Warning("Cannot reconnect to PubSub socket, currently connecting.");
                return false;
            }

            return result;
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Fired when the client successfuly connects to the socket.
        /// Handles auto listening to topics and reconnecting.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnWebSocketSharpOpen(object sender, EventArgs e)
        {
            auto_ping_timer = new Timer(PING_FREQUENCY_MS);
            auto_ping_timer.Elapsed += (_sender, _e) => PingAsync();
            SetAutoPingEnabled(true);

            pending_pong_timer = new Timer(PENDING_PONG_LIMIT_MS);
            pending_pong_timer.Elapsed += CheckPong;
            pending_pong_timer.Enabled = false;

            if (reconnecting)
            {
                LibraryDebug.Success("Reconnected to PubSub socket.");
                OnReconnected.Raise(this, EventArgs.Empty);
                reconnecting = false;

                LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Reconnection Process Completed");
                LibraryDebug.BlankLine();
            }
            else
            {
                LibraryDebug.Success("Connected to PubSub socket.");
                OnConnected.Raise(this, EventArgs.Empty);

                LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Connection Process Completed");
                LibraryDebug.BlankLine();
            }
        }

        /// <summary>
        /// Fired when the client successfuly disconnects from the socket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnWebSocketSharpClose(object sender, CloseEventArgs e)
        {
            LibraryDebug.PrintLine("Discconnected from PubSub socket.");
            auto_ping_timer.Dispose();
            pending_pong_timer.Dispose();

            if (reconnecting)
            {
                Connect();
            }
            else
            {
                auto_ping_timer.Enabled = false;
                pending_pong_timer.Enabled = false;

                OnDisconnected.Raise(this, EventArgs.Empty);

                LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Disonnection Process Completed");
                LibraryDebug.BlankLine();
            }
        }

        /// <summary>
        /// Fired when an error is received from the WebSocketSharp.dll.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnWebSocketSharpError(object sender, ErrorEventArgs e)
        {
            LibraryDebug.Error(TimeStamp.TimeLong, "Error received from WebSocketSharp.",
                               LibraryDebug.FormatAsColumns(nameof(e.Exception), e.Exception.ToString()),
                               LibraryDebug.FormatAsColumns(nameof(e.Message), e.Message));

            OnError.Raise(this, new ErrorReceivedEventArgs
            {
                exception = e.Exception,
            });
        }

        /// <summary>
        /// Fired when a MESSAGE is receieved through the socket.
        /// The message is parsed and processed according to what type it is.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private async void OnWebSocketSharpMessage(object sender, MessageEventArgs e)
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "Message Data Recieved from PubSub socket. Processing Starting...");

            #region Fake messages
            /*
            PubSubMessage fake_message = new PubSubMessage
            {
                type = PubSubType.MESSAGE.ToString(),
                data = new PubSubMessageData
                {
                    topic = string.Empty,
                    message = string.Empty
                }
            };

            // NOTE: (PubSub) Bits event cannot be tested first hand, so this fake object has been created to mimic a response assuming the documentation is correct
            // fake_message.data.topic = "channel-bits-events-v1.44322889";
            // fake_message.data.message = "{\"data\":{\"user_name\":\"dallasnchains\",\"channel_name\":\"dallas\",\"user_id\":\"129454141\",\"channel_id\":\"44322889\",\"chat_message\":\"cheer10000 New badge hype!\",\"bits_used\":10000,\"total_bits_used\":25000,\"context\":\"cheer\",\"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}},\"version\":\"1.0\",\"message_type\":\"bits_event\",\"message_id\":\"8145728a4-35f0-4cf7-9dc0-f2ef24de1eb6\"}"

            // NOTE: (PubSub) Subscription event cannot be tested first hand, so this fake object has been created to mimic a response assuming the documentation is correct
            PubSubSubscriptionsMessage fake_sub_message = new PubSubSubscriptionsMessage
            {
                user_name = "dallas",
                display_name = "dallas",
                channel_name = "twitch",
                user_id = "44322889",
                channel_id = "12826",
                time = DateTime.Parse("2015-12-19T16:39:57-08:00"),
                sub_plan = "1000",
                sub_plan_name = "Mr_Woodchuck - Channel Subscription (mr_woodchuck)",
                months = 9,
                context = "resub",
                sub_message = new PubSubSubscriptionsSubMessage
                {
                    message = "A Twitch baby is born! KappaHD",
                    emotes = new List<PubSubEmotes>
                    {
                        new PubSubEmotes
                        {
                            start = 7,
                            end = 23,
                            id = "2867"
                        }
                    }
                }
            };

            fake_message.data.topic = "channel-subscribe-events-v1.44322889";
            fake_message.data.message = JsonConvert.SerializeObject(fake_sub_message);

            string fake_message_string = JsonConvert.SerializeObject(fake_message);

            PubSubMessageType _type = await fake_message_string.TryDeserializeObjectAsync<PubSubMessageType>();
            */
            #endregion

            PubSubMessageType message_type = await e.Data.TryDeserializeObjectAsync<PubSubMessageType>();            
            Enum.TryParse(message_type.type, true, out PubSubType type);
            switch (type)
            {
                case PubSubType.PONG:
                    {
                        LibraryDebug.PrintLine("PONG recieved from PubSub");
                        pending_pong_timer.Enabled = false;

                        OnPong.Raise(this, new MessageTypeEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
                case PubSubType.RECONNECT:
                    {
                        LibraryDebug.PrintLine("RECONNECT recieved from PubSub");

                        if (auto_reconnect)
                        {
                            ReconnectAsync();
                        }

                        OnRecconect.Raise(this, new MessageTypeEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
                case PubSubType.RESPONSE:
                    {
                        LibraryDebug.PrintLine("RESPONSE recieved from PubSub");
                        // LibraryDebug.PrintObject(e.Data);

                        PubSubResponse response = await e.Data.TryDeserializeObjectAsync<PubSubResponse>();
                        OnResponse.Raise(this, new ResponseReceivedEventArgs
                        {
                            data = e.Data,
                            response = response
                        });
                    }
                    break;
                case PubSubType.MESSAGE:
                    {
                        LibraryDebug.PrintLine("MESSAGE recieved from PubSub");

                        PubSubMessage message = await e.Data.TryDeserializeObjectAsync<PubSubMessage>();
                        OnMessage.Raise(this, new MessageReceivedEventArgs
                        {
                            data = e.Data,
                            message = message
                        });

                        string topic = message.data.topic.TextBefore(".");

                        switch (topic)
                        {
                            case "whispers":
                                {
                                    LibraryDebug.PrintLine("Whisper message recieved from PubSub");

                                    PubSubWhisperMessage whisper_message = JsonConvert.DeserializeObject<PubSubWhisperMessage>(message.data.message);
                                    PubSubWhisperMessageData whisper_message_data = JsonConvert.DeserializeObject<PubSubWhisperMessageData>(whisper_message.data);

                                    OnWhisper.Raise(this, new WhisperReceivedEventArgs
                                    {
                                        data = e.Data,
                                        whisper_message = whisper_message,
                                        whisper_message_data = whisper_message_data
                                    });
                                }
                                break;
                            case "channel-bits-events-v1":
                                {
                                    LibraryDebug.PrintLine("Bits message recieved from PubSub");
                                    PubSubBitsMessage bits_message = JsonConvert.DeserializeObject<PubSubBitsMessage>(message.data.message);

                                    OnBits.Raise(this, new BitsReceivedEventArgs
                                    {
                                        data = e.Data,
                                        bits_message = bits_message
                                    });
                                }
                                break;
                            case "channel-subscribe-events-v1":
                                {
                                    LibraryDebug.PrintLine("Subscription message recieved from PubSub");
                                    PubSubSubscriptionsMessage subscription_message = JsonConvert.DeserializeObject<PubSubSubscriptionsMessage>(message.data.message);

                                    OnSubscription.Raise(this, new SubscriptionReceivedEventArgs
                                    {
                                        data = e.Data,
                                        subscription_message = subscription_message
                                    });
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        LibraryDebug.Error("Unsuported PubSub type recieved",
                                           LibraryDebug.FormatAsColumns(nameof(type), type.ToString()));

                        OnUnsupportedMessageType.Raise(this, new UnsupportedMessageTypeReceivedEventArgs
                        {
                            data = e.Data,
                            type = type
                        });
                    }
                    break;
            }

            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Message Data Processing Completed");
            LibraryDebug.BlankLine();
        }

        #endregion

        #region Listen and Unlisten

        /// <summary>
        /// Send a request to LISTEN to a singular topic.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        /// <param name="topic">The topic to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void Listen(string topic, string nonce = "")
        {
            Listen(new List<string>() { topic }, nonce);
        }

        /// <summary>
        /// Send a request to LISTEN to a <see cref="List{T}"/> of topics.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        /// <param name="topics">A <see cref="List{T}"/> of topics to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void Listen(List<string> topics, string nonce = "")
        {
            SendRequest(PubSubType.LISTEN, topics, nonce);
        }

        /// <summary>
        /// Asynchronously send a request to LISTEN to a singular topic.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        /// <param name="topic">The topic to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void ListenAsync(string topic, string nonce = "")
        {
            ListenAsync(new List<string>() { topic }, nonce);
        }

        /// <summary>
        /// Asynchronously send a request to LISTEN to a <see cref="List{T}"/> of topics.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        /// <param name="topics">A <see cref="List{T}"/> of topics to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void ListenAsync(List<string> topics, string nonce = "")
        {
            SendRequestAsync(PubSubType.LISTEN, topics, nonce);
        }

        /// <summary>
        /// Send a request to UNLISTEN from an active singular topic.
        /// </summary>
        /// <param name="topic">The topic to UNLISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void Unlisten(string topic, string nonce = "")
        {
            Unlisten(new List<string>() { topic }, nonce);
        }

        /// <summary>
        /// Send a request to UNLISTEN from a list of active topics.
        /// </summary>
        /// <param name="topics">A <see cref="List{T}"/> of topics to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void Unlisten(List<string> topics, string nonce = "")
        {
            SendRequest(PubSubType.UNLISTEN, topics, nonce);
        }

        /// <summary>
        /// Asynchronously send a request to UNLISTEN from an active singular topic.
        /// </summary>
        /// <param name="topic">The topic to UNLISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void UnlistenAsync(string topic, string nonce = "")
        {
            UnlistenAsync(new List<string>() { topic }, nonce);
        }

        /// <summary>
        /// Asynchronously send a request to UNLISTEN from a list of active topics.
        /// </summary>
        /// <param name="topics">A <see cref="List{T}"/> of topics to LISTEN to.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        public void UnlistenAsync(List<string> topics, string nonce = "")
        {
            SendRequestAsync(PubSubType.UNLISTEN, topics, nonce);
        }

        /// <summary>
        /// Sends a LISTEN or UNLISTEN request with a list of topics to the PubSub socket.
        /// </summary>
        /// <param name="type">The <see cref="PubSubType"/> (request) sent to the PubSub socket.</param>
        /// <param name="topics">The <see cref="List{T}"/> of topics associated with the request.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        private void SendRequest(PubSubType type, List<string> topics, string nonce = "")
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "Starting PubSub Request Process...", LibraryDebug.FormatAsColumns("request", type.ToString()));            

            topics = topics.Distinct().ToList();
            if(topics.Count < 1)
            {
                LibraryDebug.Warning("No new topics to reuest");
                LibraryDebug.Header(TimeStamp.TimeLong, "Request to PubSub Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            PubSubRequest request = new PubSubRequest
            {
                type = type.ToString(),
                nonce = nonce,
                data = new PubSubRequestData
                {
                    topics = topics,
                    auth_token = oauth_token
                }
            };

            string request_serialized = JsonConvert.SerializeObject(request);
            web_socket.Send(request_serialized);

            LibraryDebug.Success("Request successfully sent to PubSub");
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Request Process Completed");
            LibraryDebug.BlankLine();
        }

        /// <summary>
        /// Asynchronously sends a LISTEN or UNLISTEN request with a list of topics to the PubSub socket.
        /// </summary>
        /// <param name="type">The <see cref="PubSubType"/> (request) sent to the PubSub socket.</param>
        /// <param name="topics">The <see cref="List{T}"/> of topics associated with the request.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        private async void SendRequestAsync(PubSubType type, List<string> topics, string nonce = "")
        {
            LibraryDebug.Header(TimeStamp.TimeLong, "Starting Async PubSub Request Process...",
                                LibraryDebug.FormatAsColumns("request", type.ToString()));

            topics = topics.AsParallel().Distinct().ToList();
            if (topics.Count < 1)
            {
                LibraryDebug.Warning("No new topics to reuest");
                LibraryDebug.Header(TimeStamp.TimeLong, "Request to PubSub Aborted");
                LibraryDebug.BlankLine();
                return;
            }

            PubSubRequest request = new PubSubRequest
            {
                type = type.ToString(),
                nonce = nonce,
                data = new PubSubRequestData
                {
                    topics = topics,
                    auth_token = oauth_token
                }
            };

            string request_serialized = await request.SerializeObjectAsync();
            web_socket.SendAsync(request_serialized, RequestAsyncCallback);
        }

        private void RequestAsyncCallback(bool completed)
        {
            if (completed)
            {
                LibraryDebug.Success("Request successfully sent to PubSub");
            }
            else
            {
                LibraryDebug.Error("Failed to send request to PubSub");
            }
            
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub Request Process Completed");
            LibraryDebug.BlankLine();
        }
        #endregion

        #region Ping

        /// <summary>
        /// Enable or disable whether to automatically send a PING to the socket every 4 minutes.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetAutoPingEnabled(bool enabled)
        {
            auto_ping_timer.Enabled = enabled;

            if (!enabled)
            {
                pending_pong_timer.Enabled = false;
            }
        }

        /// <summary>
        /// Sends a PING to the socket.
        /// </summary>
        public void Ping()
        {
            // native Ping() in the WebSocketSharp doesn't operate as requested by Twitch, send the raw string ourselves
            LibraryDebug.Header(TimeStamp.TimeLong, "PubSub PING Process Starting...");
            web_socket.Send("{\"type\": \"PING\"}");

            FinishPing();
        }

        /// <summary>
        /// Asynchronously sends a PING to the socket.
        /// </summary>
        public void PingAsync()
        {
            // native Ping() in the WebSocketSharp doesn't operate as requested by Twitch, send the raw string ourselves
            LibraryDebug.Header(TimeStamp.TimeLong, "Sending Async PING to PubSub...");
            web_socket.SendAsync("{\"type\": \"PING\"}", PingCallback);
        }

        /// <summary>
        /// Called when a <see cref="PingAsync"/> has been issued.
        /// Handles how to clean up/process the PING.
        /// </summary>
        /// <param name="completed"></param>
        private void PingCallback(bool completed)
        {
            if (completed)
            {
                FinishPing();
            }
            else
            {
                LibraryDebug.Error("Failed to send async PING to PubSub.");
                OnPingFailed.Raise(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the <see cref="ping_sent_time"/> to the current <see cref="DateTime"/>.
        /// Starts the <see cref="pending_pong_timer"/> to wait for the PONG from the socket.
        /// </summary>
        private void FinishPing()
        {
            ping_sent_time = DateTime.Now;
            pending_pong_timer.Enabled = true;

            LibraryDebug.Success("PING successfuly sent to PubSub");
            OnPing.Raise(this, EventArgs.Empty);

            LibraryDebug.Header("PubSub PING Processes Completed");
            LibraryDebug.BlankLine();
        }

        /// <summary>
        /// Checks to see when the PONG was received from Twitch.
        /// Issues a <see cref="ReconnectAsync"/> if a PONG has not been received 10 seconds after a successful PING.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void CheckPong(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now - ping_sent_time > TimeSpan.FromMilliseconds(PENDING_PONG_LIMIT_MS))
            {
                LibraryDebug.Warning(TimeStamp.TimeLong, "PING sent to PubSub but did not receive a PONG after 10s. Issuing reconnect...");
                LibraryDebug.BlankLine();
                ReconnectAsync();
            }
        }

        #endregion
    }
}