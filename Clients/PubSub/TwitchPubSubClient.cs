﻿// standard namespaces
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
using TwitchLibrary.Models.Clients.PubSub.Request;

// imported .dll's
using Newtonsoft.Json;
using WebSocketSharp;

namespace TwitchLibrary.Clients.PubSub
{
    public class TwitchPubSubClient
    {
        #region Fields

        // private

        private bool                                                        reconnecting;

        private readonly int                                                PENDING_PONG_LIMIT_MS = 10 * 1000;
        private readonly int                                                PING_FREQUENCY_MS = 4 * 60 * 1000;

        private readonly string                                             WEB_SOCKET = "wss://pubsub-edge.twitch.tv";
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
        public event EventHandler<TypeEventArgs>                            OnRecconect;

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
        public event EventHandler<TypeEventArgs>                            OnPong;

        /// <summary>
        /// Raised when a user has cheered with bits.
        /// </summary>
        public event EventHandler<BitsEventArgs>                            OnBits;

        /// <summary>
        /// Raised when a MESSAGE has been received through the socket.
        /// </summary>
        public event EventHandler<Events.Clients.PubSub.MessageEventArgs>   OnMessage;

        /// <summary>
        /// Raised when a user has subscribed to a channel.
        /// </summary>
        public event EventHandler<SubscriberEventArgs>                      OnSubscription;

        /// <summary>
        /// Raised when a whisper has been received by a user.
        /// </summary>
        public event EventHandler<WhisperEventArgs>                         OnWhisper;

        /// <summary>
        /// Raised when a user buys a product through another channel.
        /// </summary>
        public event EventHandler<CommerceEventArgs>                        OnCommerce;

        /// <summary>
        /// Raised when a RESPONSE has been receieved through the socket.
        /// </summary>
        public event EventHandler<ResponseEventArgs>                        OnResponse;

        #endregion

        #region Constructors

        public TwitchPubSubClient(string _oauth_token, string _id = "")
        {
            auto_reconnect          = true;
            reconnecting            = false;

            id                      = _id;
            oauth_token             = _oauth_token;                       

            web_socket =            new WebSocket(WEB_SOCKET);
            web_socket.OnOpen       += new EventHandler(OnWebSocketSharpOpen);
            web_socket.OnClose      += new EventHandler<CloseEventArgs>(OnWebSocketSharpClose);
            web_socket.OnMessage    += new EventHandler<WebSocketSharp.MessageEventArgs>(OnWebSocketSharpMessage);
            web_socket.OnError      += new EventHandler<ErrorEventArgs>(OnWebSocketSharpError);
        }

        #endregion

        #region Connection methods

        /// <summary>
        /// Asynchronously connects to the web socket.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        public void ConnectAsync()
        {
            Log.Header(TimeStamp.TimeLong, "PubSub Async Connection Process Starting...");

            if (!CanConnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Async Connection Process Aborted");
                Log.BlankLine();
                return;
            }

            ping_sent_time = DateTime.Now;

            Log.PrintLine("Asynchronously connecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.ConnectAsync();
        }

        /// <summary>
        /// Connects to the web socket.
        /// At least one topic must be listend to witin 15 of establishing a successful connection.
        /// </summary>
        public void Connect()
        {
            Log.Header(TimeStamp.TimeLong, "PubSub Connection Process Starting...");

            if (!CanConnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Connection Process Aborted");
                Log.BlankLine();
                return;
            }

            ping_sent_time = DateTime.Now;

            Log.PrintLine("Connecting to PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
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
                Log.Warning("Cannot connect to PubSub socket, already connected.");
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Asynchronously disconnects from the web socket.
        /// </summary>
        public void DisconnectAsync()
        {
            Log.Header(TimeStamp.TimeLong, "PubSub Async Disonnection Process Starting...");

            if (!CanDisconnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Async Disconnection Process Aborted");
                Log.BlankLine();
                return;
            }

            Log.PrintLine("Asynchronously disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
            web_socket.CloseAsync();
        }

        /// <summary>
        /// Disconnects from the web socket.
        /// </summary>
        public void Disconnect()
        {
            Log.Header(TimeStamp.TimeLong, "PubSub Disonnection Process Starting...");

            if (!CanDisconnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Disconnection Process Aborted");
                Log.BlankLine();
                return;
            }

            Log.PrintLine("Disconnecting from PubSub socket " + WEB_SOCKET.Wrap("\"", "\"") + "...");
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
                Log.Warning("Cannot discconect from PubSub socket, already disconnected.");
                result = false;
            } 
            else if (web_socket.ReadyState == WebSocketState.Closing)
            {
                Log.Warning("Cannot discconect from PubSub socket, already disconnecting.");
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
            Log.Header(TimeStamp.TimeLong, "PubSub Async Reconnection Process Starting...");

            if (!CanReconnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Async Reconnection Process Aborted");
                Log.BlankLine();
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
            Log.Header(TimeStamp.TimeLong, "PubSub Reconnection Process Starting...");

            if (!CanReconnect())
            {
                Log.Error(TimeStamp.TimeLong, "PubSub Reconnection Process Aborted.");
                Log.BlankLine();
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
                Log.Warning("Cannot recconect to PubSub socket, currently disconnecting.");
                return false;
            }
            else if (web_socket.ReadyState == WebSocketState.Connecting)
            {
                Log.Warning("Cannot reconnect to PubSub socket, currently connecting.");
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
                Log.Success("Reconnected to PubSub socket.");
                OnReconnected.Raise(this, EventArgs.Empty);
                reconnecting = false;

                Log.Header(TimeStamp.TimeLong, "PubSub Reconnection Process Completed");
                Log.BlankLine();
            }
            else
            {
                Log.Success("Connected to PubSub socket.");
                OnConnected.Raise(this, EventArgs.Empty);

                Log.Header(TimeStamp.TimeLong, "PubSub Connection Process Completed");
                Log.BlankLine();
            }
        }

        /// <summary>
        /// Fired when the client successfuly disconnects from the socket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnWebSocketSharpClose(object sender, CloseEventArgs e)
        {
            Log.PrintLine("Discconnected from PubSub socket.");
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

                Log.Header(TimeStamp.TimeLong, "PubSub Disonnection Process Completed");
                Log.BlankLine();
            }
        }

        /// <summary>
        /// Fired when an error is received from the WebSocketSharp.dll.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnWebSocketSharpError(object sender, ErrorEventArgs e)
        {
            Log.Error(TimeStamp.TimeLong, "Error received from WebSocketSharp.",
                               Log.FormatColumns(nameof(e.Exception), e.Exception.ToString()),
                               Log.FormatColumns(nameof(e.Message), e.Message));

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
        private async void OnWebSocketSharpMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            Log.Header(TimeStamp.TimeLong, "Message Data Recieved from PubSub socket. Processing Starting...");

            #region Fake messages

            //PubSubMessage fake_message = new PubSubMessage
            //{
            //    type = PubSubType.MESSAGE.ToString(),
            //    data = new PubSubMessageData
            //    {
            //        topic = string.Empty,
            //        message = string.Empty
            //    }
            //};

            // NOTE: (PubSub) Bits event cannot be tested first hand, so this fake object has been created to mimic a response assuming the documentation is correct
            // fake_message.data.topic = "channel-bits-events-v1.44322889";
            // fake_message.data.message = "{\"data\":{\"user_name\":\"dallasnchains\",\"channel_name\":\"dallas\",\"user_id\":\"129454141\",\"channel_id\":\"44322889\",\"chat_message\":\"cheer10000 New badge hype!\",\"bits_used\":10000,\"total_bits_used\":25000,\"context\":\"cheer\",\"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}},\"version\":\"1.0\",\"message_type\":\"bits_event\",\"message_id\":\"8145728a4-35f0-4cf7-9dc0-f2ef24de1eb6\"}"

            // NOTE: (PubSub) Subscription event cannot be tested first hand, so this fake object has been created to mimic a response assuming the documentation is correct
            //PubSubSubscriptionsMessage fake_sub_message = new PubSubSubscriptionsMessage
            //{
            //    user_name = "dallas",
            //    display_name = "dallas",
            //    channel_name = "twitch",
            //    user_id = "44322889",
            //    channel_id = "12826",
            //    time = DateTime.Parse("2015-12-19T16:39:57-08:00"),
            //    sub_plan = "1000",
            //    sub_plan_name = "Mr_Woodchuck - Channel Subscription (mr_woodchuck)",
            //    months = 9,
            //    context = "resub",
            //    sub_message = new PubSubSubscriptionsSubMessage
            //    {
            //        message = "A Twitch baby is born! KappaHD",
            //        emotes = new List<PubSubEmotes>
            //        {
            //            new PubSubEmotes
            //            {
            //                start = 7,
            //                end = 23,
            //                id = "2867"
            //            }
            //        }
            //    }
            //};

            //PubSubCommerceMessage fake_commerce_message = new PubSubCommerceMessage
            //{
            //    user_name = "dallas",
            //    display_name = "dallas",
            //    channel_name = "twitch",
            //    user_id = "44322889",
            //    channel_id = "12826",
            //    time = DateTime.Parse("2015-12-19T16:39:57-08:00"),
            //    item_image_url = "url here",
            //    item_description = "This is a friendly description!",
            //    supports_channel = true,
            //    purchase_message = new PubSubCommercePurchaseMessage
            //    {
            //        message = "A Twitch game is born! Kappa",
            //        emotes = new List<PubSubEmotes>()
            //        {
            //            new PubSubEmotes()
            //            {
            //                start = 23,
            //                end = 7,
            //                id = "2867"
            //            }
            //        }
            //    }
            //};

            //fake_message.data.topic = "channel-commerce-events-v1.44322889";
            //fake_message.data.message = JsonConvert.SerializeObject(fake_commerce_message);

            //string fake_message_string = JsonConvert.SerializeObject(fake_message);

            #endregion

            PubSubMessage pub_sub_message = await e.Data.TryDeserializeObjectAsync<PubSubMessage>();                          //FOR SHIPPING
            //PubSubMessage pub_sub_message = await fake_message_string.TryDeserializeObjectAsync<PubSubMessage>();       //FOR TESTING
            Enum.TryParse(pub_sub_message.type, true, out PubSubType type);
            switch (type)
            {
                case PubSubType.PONG:
                    {
                        Log.PrintLine("PONG recieved from PubSub");

                        pending_pong_timer.Enabled = false;

                        OnPong.Raise(this, new TypeEventArgs(e.Data, type));
                    }
                    break;
                case PubSubType.RECONNECT:
                    {
                        Log.PrintLine("RECONNECT recieved from PubSub");

                        if (auto_reconnect)
                        {
                            ReconnectAsync();
                        }

                        OnRecconect.Raise(this, new TypeEventArgs(e.Data, type));
                    }
                    break;
                case PubSubType.RESPONSE:
                    {
                        Log.PrintLine("RESPONSE recieved from PubSub");

                        OnResponse.Raise(this, new ResponseEventArgs(e.Data));
                    }
                    break;
                case PubSubType.MESSAGE:
                    {
                        Log.PrintLine("MESSAGE recieved from PubSub");

                        OnMessage.Raise(this, new Events.Clients.PubSub.MessageEventArgs(e.Data, pub_sub_message));

                        string topic = pub_sub_message.data.topic.TextBefore(".");

                        switch (topic)
                        {
                            case "whispers":
                                {
                                    Log.PrintLine("Whisper message recieved from PubSub");

                                    OnWhisper.Raise(this, new WhisperEventArgs(e.Data, pub_sub_message));
                                }
                                break;
                            case "channel-bits-events-v1":
                                {
                                    Log.PrintLine("Bits message recieved from PubSub");

                                    OnBits.Raise(this, new BitsEventArgs(e.Data, pub_sub_message));
                                }
                                break;
                            case "channel-subscribe-events-v1":
                                {
                                    Log.PrintLine("Subscription message recieved from PubSub");

                                    OnSubscription.Raise(this, new SubscriberEventArgs(e.Data, pub_sub_message));
                                }
                                break;
                            case "channel-commerce-events-v1":
                                {
                                    Log.PrintLine("Commerce message recieved from PubSub");

                                    OnCommerce.Raise(this, new CommerceEventArgs(e.Data, pub_sub_message));
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        Log.Error("Unsuported PubSub type recieved",
                                           Log.FormatColumns(nameof(type), type.ToString()));
                    }
                    break;
            }

            Log.Header(TimeStamp.TimeLong, "PubSub Message Data Processing Completed");
            Log.BlankLine();
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
            Log.Header(TimeStamp.TimeLong, "Starting PubSub Request Process...", Log.FormatColumns("request", type.ToString()));            

            topics = topics.Distinct().ToList();
            if(topics.Count < 1)
            {
                Log.Warning("No new topics to reuest");
                Log.Header(TimeStamp.TimeLong, "Request to PubSub Aborted");
                Log.BlankLine();
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

            Log.Success("Request successfully sent to PubSub");
            Log.Header(TimeStamp.TimeLong, "PubSub Request Process Completed");
            Log.BlankLine();
        }

        /// <summary>
        /// Asynchronously sends a LISTEN or UNLISTEN request with a list of topics to the PubSub socket.
        /// </summary>
        /// <param name="type">The <see cref="PubSubType"/> (request) sent to the PubSub socket.</param>
        /// <param name="topics">The <see cref="List{T}"/> of topics associated with the request.</param>
        /// <param name="nonce">An optional unique <see cref="string"/> to identiy the response associated with the request.</param>
        private async void SendRequestAsync(PubSubType type, List<string> topics, string nonce = "")
        {
            Log.Header(TimeStamp.TimeLong, "Starting Async PubSub Request Process...",
                                Log.FormatColumns("request", type.ToString()));

            topics = topics.AsParallel().Distinct().ToList();
            if (topics.Count < 1)
            {
                Log.Warning("No new topics to reuest");
                Log.Header(TimeStamp.TimeLong, "Request to PubSub Aborted");
                Log.BlankLine();
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
                Log.Success("Request successfully sent to PubSub");
            }
            else
            {
                Log.Error("Failed to send request to PubSub");
            }
            
            Log.Header(TimeStamp.TimeLong, "PubSub Request Process Completed");
            Log.BlankLine();
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
            Log.Header(TimeStamp.TimeLong, "PubSub PING Process Starting...");
            web_socket.Send("{\"type\": \"PING\"}");

            FinishPing();
        }

        /// <summary>
        /// Asynchronously sends a PING to the socket.
        /// </summary>
        public void PingAsync()
        {
            // native Ping() in the WebSocketSharp doesn't operate as requested by Twitch, send the raw string ourselves
            Log.Header(TimeStamp.TimeLong, "Sending Async PING to PubSub...");
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
                Log.Error("Failed to send async PING to PubSub.");
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

            Log.Success("PING successfuly sent to PubSub");
            OnPing.Raise(this, EventArgs.Empty);

            Log.Header("PubSub PING Processes Completed");
            Log.BlankLine();
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
                Log.Warning(TimeStamp.TimeLong, "PING sent to PubSub but did not receive a PONG after 10s. Issuing reconnect...");
                Log.BlankLine();
                ReconnectAsync();
            }
        }

        #endregion
    }
}