using System.Collections.Generic;
using XenForms.Core.Diagnostics;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Core.Toolbox
{
    public abstract class ToolboxLogging : Logging
    {
        public abstract IEnumerable<LogItem> LiveLogItems { get; }
        public abstract string[] LiveLog { get; }
        public abstract void ClearLiveLog();
        public abstract void ConfigureTraceLog(ISettingsStore store);
        public abstract string[] GetDetailedLogInformation(string extra);
    }
}