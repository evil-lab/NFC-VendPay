using System;
using System.ComponentModel;

namespace com.IntemsLab.Server.Network
{
    public static class EventHandlerExtensions
    {
        public static void Fire(this EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        public static void Fire<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        public static void Fire(this PropertyChangedEventHandler handler, object sender, string propertyName)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}