using System;
using PubSub;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Shell
{
    public class DefaultMessageBus : IMessageBus
    {
        public void Notify<T>(T data) where T : IAppEvent
        {
            this.Publish(data);
        }


        public void Listen<T>(Action<T> handler) where T : IAppEvent
        {
            this.Subscribe(handler);
        }


        public void StopListening<T>() where T : IAppEvent
        {
            this.Unsubscribe<T>();
        }
    }
}