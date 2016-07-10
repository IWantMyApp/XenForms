using System;
using System.IO;
using System.Threading;
using Eto.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Toolbox.UI.Pouches.FilesFolder.Commands
{
    [MenuPlacement("80f4ca8b-d04b-48c4-89c1-f6c184fa3b00", MenuLocation.FilesFolders, "Designer")]
    public class WatchXamlCommand : FilesFolderCommand
    {
        private readonly object _lock = new object();
        private string _watchingFile = string.Empty;
        private DateTime _lastWriteTime;
        private Timer _timer;


        public WatchXamlCommand(ProjectWorkspace project, IMessageBus messaging, ToolboxSocket socket) 
            : base(project, messaging, socket)
        {
            MenuText = "Watch for Changes";
            ToolTip = "Continually send external changes made to the XAML file to the designer.";
        }


        public override MenuItem CreateMenuItem()
        {
            var menu = base.CreateMenuItem();

            menu.Validate += (s, e) =>
            {
                menu.Enabled = Project.Supports.IsXaml(SelectedFile) && Socket.IsConnected;
            };

            return menu;
        }


        private void OnFileWriteTimeCheck(object state)
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_watchingFile)) return;

                    if (SelectedFile == null) return;

                    var now = File.GetLastWriteTime(SelectedFile);
                    if (now == _lastWriteTime) return;
                    _lastWriteTime = now;

                    Messaging.Notify(new ShowStatusMessage($"Reloading {SelectedFile}"));

                    var msg = XenMessage.Create<OpenXamlRequest>();
                    msg.Xaml = File.ReadAllText(_watchingFile);
                    Socket.Send(msg);
                }
                catch (Exception)
                {
                    lock (_lock)
                    {
                        _timer.Dispose();
                    }
                }
            }
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            if (!File.Exists(SelectedFile))
            {
                return;
            }

            if (_timer == null)
            {
                lock (_lock)
                {
                    _timer = new Timer(OnFileWriteTimeCheck, null, 0, 5000);
                }
            }

            lock (_lock)
            {
                _watchingFile = SelectedFile;
                _lastWriteTime = File.GetLastWriteTime(SelectedFile);
            }
        }
    }
}
