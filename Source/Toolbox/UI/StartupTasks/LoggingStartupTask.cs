using System.Threading.Tasks;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.StartupTasks
{
    public class LoggingStartupTask : IStartupTask
    {
        private readonly ISettingsStore _store;

        public int ExecutionOrder { get; } = 1;
        public ExecuteWhen ExecuteWhen { get; } = ExecuteWhen.ApplicationInitialized;


        public LoggingStartupTask(ISettingsStore store)
        {
            _store = store;
        }


        public Task<bool> ExecuteAsync(object data)
        {
            ToolboxApp.Log.ConfigureTraceLog(_store);
            return Task.FromResult(true);
        }
    }
}