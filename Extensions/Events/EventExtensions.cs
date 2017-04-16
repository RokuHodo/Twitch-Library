//standard namespaces
using System;
using System.Threading.Tasks;

using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;

namespace TwitchLibrary.Extensions.Events
{
    public static class EventExtensions
    {
        /// <summary>
        /// Asynchronously raises an <see cref="EventHandler"/>.
        /// This extension method should only be used to raise a CPU bound <see cref="EventHandler"/>.
        /// </summary>
        /// <typeparam name="type">The implied type of the <see cref="EventHandler"/>.</typeparam>
        /// <param name="handler">The <see cref="EventHandler"/> to raise asynchronously.</param>
        /// <param name="sender">The sender that raised the <see cref="EventHandler"/>.</param>
        /// <param name="args">The <see cref="EventArgs"/> to be passed through with the event.</param>
        public static void RaiseAsync<type>(this EventHandler<type> handler, object sender, type args) where type : EventArgs
        {
            if (handler.isNull())
            {
                return;
            }

            try
            {
                Task task = Task.Run(() =>
                {
                    handler(sender, args);
                });

                task.Wait();

                Task continuation = task.ContinueWith(cont =>
                {
                    {
                        LibraryDebug.Error(LibraryDebugMethod.RAISE, "event", LibraryDebugError.NORMAL_FAULTED);
                        LibraryDebug.PrintLine(nameof(type), typeof(type).Name);
                        LibraryDebug.PrintLine(nameof(cont.Exception), cont.Exception.Message);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

                LibraryDebug.Notify(handler.Method.Name +  " raise finished", TimeStamp.TimeLong);
            }
            catch(Exception exception)
            {
                //in case something really weird happened 
                LibraryDebug.Error(LibraryDebugMethod.RAISE, "event", LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLine(nameof(type), typeof(type).Name);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }
        }
    }
}
