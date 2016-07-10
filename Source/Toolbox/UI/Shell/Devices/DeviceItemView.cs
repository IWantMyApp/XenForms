using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core;
using XenForms.Core.Networking;
using XenForms.Core.Platform;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.Devices
{
    public class DeviceItemView : Panel
    {
        private Label _ulabel;
        private Label _slabel;
        private ImageButton _startBtn;
        private ImageButton _uploadBtn;
        public AndroidDeviceBridge Bridge { get; }
        public MobileDevice Device { get; }
        public Panel StatusPanel { get; } = new Panel();


        public DeviceItemView(AndroidDeviceBridge bridge, MobileDevice device)
        {
            Bridge = bridge;
            Device = device;
            CreateLayout();
        }


        private void CreateLayout()
        {
            Tag = Device;
            BackgroundColor = new Color(SystemColors.WindowBackground, 0.7f);
            Size = new Size(750, 85);

            SetStatus(Device.Model, 16);

            var container = new TableLayout(3, 1)
            {
                Padding = new Padding(15)
            };

            container.Add(CreateReady(), 0, 0, false, false);
            container.Add(StatusPanel, 1, 0, true, false);
            container.Add(CreateActions(), 2, 0, false, false);

            Content = container;
        }


        private Control CreateReady()
        {
            var color = Device.Status == "Online" ? Colors.Green : Colors.Black;

            return new Label
            {
                Font = new Font(SystemFont.Default, 14),
                Text = Device.Status,
                TextColor = color,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
        }


        private void SetStatus(string text, int size)
        {
            StatusPanel.Content = new Label
            {
                Font = new Font(SystemFont.Default, size),
                Text = text,
                TextColor = new Color(SystemColors.ControlText, .6f),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
        }


        private Control CreateActions()
        {
            var container = new StackLayout
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Spacing = 10
            };

            var ustack = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Width = 55
            };

            _uploadBtn = new ImageButton
            {
                ToolTip = "Install / Update Designer",
                Size = new Size(24, 24),
                Image = AppImages.Download
            };

            _ulabel = new Label
            {
                TextColor = new Color(SystemColors.ControlText, .6f),
                Text = "Checking"
            };

            ustack.Items.Add(_uploadBtn);
            ustack.Items.Add(_ulabel);

            var sstack = new StackLayout
            {
                Spacing = 5,
                Orientation = Orientation.Vertical,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Width = 55
            };

            _startBtn = new ImageButton
            {
                ToolTip = "Start Designer & Connect",
                Size = new Size(24, 24),
                Image = AppImages.Launch
            };

            _slabel = new Label
            {
                TextColor = new Color(SystemColors.ControlText, .6f),
                Text = "Checking"
            };

            CheckIfDesignerInstalled();

            sstack.Items.Add(_startBtn);
            sstack.Items.Add(_slabel);

            container.Items.Add(ustack);
            container.Items.Add(sstack);

            _uploadBtn.Click += UploadOnClick;
            _startBtn.Click += StartOnClick;
            return container;
        }


        private void CheckIfDesignerInstalled()
        {
            Application.Instance.AsyncInvoke(async () =>
            {
                try
                {
                    ToolboxApp.Log.Info($"Checking if designer is installed on {Device.Address}.");

                    var installed = await Bridge.CheckDesignerInstalledAsync(Device);
                    var current = await Bridge.IsRecentVersionInstalledAsync(Device, XenFormsEnvironment.DesignerVersion);

                    if (installed)
                    {
                        if (current)
                        {
                            _ulabel.Text = "Reinstall";
                            _slabel.Text = "Launch";
                            _startBtn.Enabled = true;
                        }
                        else
                        {
                            _ulabel.Text = "Update";
                            _slabel.Text = "Disabled";
                            _startBtn.Enabled = false;
                        }
                    }
                    else
                    {
                        _ulabel.Text = "Install";
                        _slabel.Text = "Disabled";
                        _startBtn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    SetStatus("Error validating designer installation. Please check the logs.", 12);
                    ToolboxApp.Log.Error("Error occurred when checking if designer was installed.", ex);
                }
            });
        }


        private async void UploadOnClick(object sender, EventArgs e)
        {
            try
            {
                _startBtn.Enabled = false;
                _uploadBtn.Enabled = false;

                ToolboxApp.Log.Info($"Starting install of {AndroidDeviceBridge.ApkName}");
                SetStatus("Installing XenForms Desginer...", 14);

                var path = Path.Combine(AssemblyDirectory, AndroidDeviceBridge.ApkName);
                await Bridge.InstallDesignerAsync(Device, path);

                ToolboxApp.Log.Info($"Confirming package {AndroidDeviceBridge.ApkName} was pushed.");
                SetStatus("Validating installation...", 14);

                var installed = await Bridge.CheckDesignerInstalledAsync(Device);

                if (installed)
                {
                    SetStatus("Installation complete!", 14);
                    ToolboxApp.Log.Info($"{AndroidDeviceBridge.ApkName} installation successful.");

                    CheckIfDesignerInstalled();
                    await Task.Delay(750);
                    SetStatus(Device.Product, 16);
                }
                else
                {
                    SetStatus("Installation failed. Check the logs for more information.", 14);
                    ToolboxApp.Log.Error($"{AndroidDeviceBridge.ApkName} did not successfully install.");
                }

                _startBtn.Enabled = true;
                _uploadBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                SetStatus("Installation failed. Please check the logs.", 12);
                ToolboxApp.Log.Error("The designer installation failed.", ex);
            }
        }


        private async void StartOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string ip;

                ToolboxApp.Log.Info($"Launching designer on {Device.Address}.");
                await Bridge.LaunchDesignerAsync(Device);

                if (Device.IsEmulator)
                {
                    ip = Device.Address;
                }
                else
                {
                    ip = await Bridge.GetConnectedDeviceIpAddressAsync(Device);
                }

                ToolboxApp.SocketManager.Connect(ip, ServiceEndpoint.Port, OnConnect, OnTrace);
            }
            catch (Exception ex)
            {
                SetStatus("The designer failed to launch. Please check the logs.", 12);
                ToolboxApp.Log.Error("Error launching designer.", ex);
            }
        }


        private void OnConnect()
        {    
            // ignored
        }


        private void OnTrace(string message)
        {
            Application.Instance.AsyncInvoke(() =>
            {
                if (ToolboxApp.IsTerminating) return;
                SetStatus(message, 14);
            });
        }


        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}