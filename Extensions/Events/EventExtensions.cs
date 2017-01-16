using System;

namespace TwitchLibrary.Extensions.Events
{
    public static class EventExtensions
    {
        public static void Raise<type>(this EventHandler<type> handler, object sender, type args)
        {
            if (handler.isNull())
            {
                return;
            }

            handler(sender, args);
        }
    }
}
