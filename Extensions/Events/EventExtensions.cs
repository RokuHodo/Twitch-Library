// standard namespaces
using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

// project namespaces
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
        public static void Raise<type>(this EventHandler<type> handler, object sender, type args) where type : EventArgs
        {
            if (handler.isNull())
            {
                return;
            }

            Delegate[] listeners = handler.GetInvocationList();
            Task[] tasks = new Task[listeners.Length];

            LibraryDebug.Notify(handler.Method.Name + " Raise Starting...");

            foreach (Delegate listener in listeners)
            {
                EventHandler<type> method = (EventHandler<type>)listener;
                string method_name = method.Method.Name.Wrap("\"", "\"");
                LibraryDebug.PrintLine("Calling " + method_name + "...");

                try
                {
                    method(sender, args);
                    LibraryDebug.PrintLine(method_name + " successfully called");
                }
                catch(Exception exception)
                {
                    LibraryDebug.Error(LibraryDebugMethod.RAISE, method_name, LibraryDebugError.NORMAL_EXCEPTION);
                    LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
                }
            }

            LibraryDebug.Notify(handler.Method.Name + " raise completed.");
        }

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

            Delegate[] listeners = handler.GetInvocationList();

            foreach(Delegate listener in listeners)
            {                
                EventHandler<type> method = (EventHandler<type>)listener;
                
                method.BeginInvoke(sender, args, EndAsyncRaise<type>, null);
                // LibraryDebug.Notify(method.Method.Name + " raise starting...", TimeStamp.TimeLong);
            }
        }

        /// <summary>
        /// Ends the invoke of an <see cref="EventHandler"/>.
        /// </summary>
        /// <typeparam name="type">The implied type of the <see cref="EventHandler"/>.</typeparam>
        /// <param name="i_result">The async result invoke.</param>
        private static void EndAsyncRaise<type>(IAsyncResult i_result)
        {
            AsyncResult result = (AsyncResult)i_result;
            EventHandler<type> method = (EventHandler<type>)result.AsyncDelegate;

            try
            {
                method.EndInvoke(i_result);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.RAISE, "event", LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLineFormatted(nameof(type), typeof(type).Name);
                LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
            }

            // LibraryDebug.Notify(method.Method.Name + " raise finished", TimeStamp.TimeLong);
        }
    }
}
