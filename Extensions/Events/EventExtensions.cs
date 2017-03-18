//standard namespaces
using System;
using System.Threading.Tasks;

namespace TwitchLibrary.Extensions.Events
{
    public static class EventExtensions
    {
        public static void RaiseAsync<type>(this EventHandler<type> handler, object sender, type args)
        {
            if (handler.isNull())
            {
                return;
            }

            Task.Run(() => handler(sender, args));
        }
    }
}
