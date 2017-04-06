//standard namespaces
using System;
using System.Threading.Tasks;

using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;

namespace TwitchLibrary.Extensions.Events
{
    public static class EventExtensions
    {
        public static void RaiseAsync<type>(this EventHandler<type> handler, object sender, type args) where type : EventArgs
        {
            if (handler.isNull())
            {
                return;
            }

            Task task = Task.Run(() =>
            {
                handler(sender, args);
            });
            task.Wait();

            if (task.IsFaulted)
            {
                LibraryDebug.Error(LibraryDebugMethod.RAISE, "event", LibraryDebugError.NORMAL_FAULTED);
                LibraryDebug.PrintLine(nameof(type), typeof(type).Name);
                LibraryDebug.PrintLine(nameof(task.Exception), task.Exception.Message);
            }
        }
    }
}
