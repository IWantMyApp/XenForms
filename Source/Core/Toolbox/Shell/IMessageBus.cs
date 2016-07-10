using System;
using XenForms.Core.Toolbox.AppEvents;

namespace XenForms.Core.Toolbox.Shell
{
    public interface IMessageBus
    {
        void Notify<T>(T data) where T : IAppEvent;
        void Listen<T>(Action<T> handler) where T : IAppEvent;
        void StopListening<T>() where T : IAppEvent;
    }
}