using System.Threading.Tasks;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Shell.MenuItems;

namespace XenForms.Toolbox.UI.StartupTasks
{
    public class ShowConnectDialogTask : IStartupTask
    {
        private readonly ISettingsStore _store;
        private readonly ConnectCommand _command;

        public int ExecutionOrder { get; } = int.MaxValue;
        public ExecuteWhen ExecuteWhen { get; } = ExecuteWhen.ApplicationInitialized;


        public ShowConnectDialogTask(ISettingsStore store, ConnectCommand command)
        {
            _store = store;
            _command = command;
        }


        public Task<bool> ExecuteAsync(object data)
        {
            var result = _store.GetBool(UserSettingKeys.Builtin.ShowConnectOnStart);
            if (result == null) return Task.FromResult(false);

            if (result.Value)
            {
                _command.Execute();
            }

            return Task.FromResult(true);
        }
    }
}
