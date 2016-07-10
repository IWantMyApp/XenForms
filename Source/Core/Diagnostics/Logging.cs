using System;

namespace XenForms.Core.Diagnostics
{
    public abstract class Logging
    {
        public abstract void Event(XenLogLevel level, string message, params object[] args);
        public abstract void Event(Exception ex, string message, params object[] args);

        public abstract void Info(string message, params object[] args);
        public abstract void Trace(string message, params object[] args);
        public abstract void Warn(string message, params object[] args);
        public abstract void Error(string message, params object[] args);
        public abstract void Error(Exception ex, string message, params object[] args);
    }
}