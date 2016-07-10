using System;
using System.ComponentModel;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell
{
    public class ConnectedDialog : Dialog
    {
        public ConnectedDialog()
        {
            Icon = AppImages.Xf;
            ToolboxApp.SocketManager.Socket.Disconnected += OnDisconnect;
            Closing += OnClosing;
        }


        private void OnDisconnect(object sender, EventArgs e)
        {
            Application.Instance.Invoke(Close);
        }


        private void OnClosing(object sender, CancelEventArgs e)
        {
            ToolboxApp.SocketManager.Socket.Disconnected -= OnDisconnect;
        }
    }
}