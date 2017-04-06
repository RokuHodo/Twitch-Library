namespace TwitchLibrary.Enums.Clients.PubSub
{
    public enum PubSubType
    {
        UNSUPPORTED = 0,
        PING,
        PONG,
        LISTEN,
        UNLISTEN,
        RECONNECT,
        MESSAGE,
        RESPONSE
    }
}
