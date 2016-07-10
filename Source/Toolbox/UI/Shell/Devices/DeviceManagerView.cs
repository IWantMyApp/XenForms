using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.Devices
{
    public class DeviceManagerView : XenVisualElement
    {
        private bool _promptDesignerRestart;
        private AndroidDeviceBridge _bridge;
        private MobileDevice[] _devices;
        private StackLayout _groups;


        public DeviceManagerView(bool promptDesignerRestart)
        {
            _promptDesignerRestart = promptDesignerRestart;
        }


        protected override Control OnDefineLayout()
        {
            _groups = new StackLayout
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
                Spacing = 50
            };

            var centered = new DynamicLayout();
            centered.AddCentered(_groups);

            CreateGroups();

            var scrollable = new Scrollable
            {
                ExpandContentWidth = true,
                ExpandContentHeight = true,
                Border = BorderType.None,
                Padding = new Padding(10),
                Content = centered
            };

            return scrollable;
        }


        private void CreateGroups()
        {
            _groups.Items.Clear();

            if (Initialize())
            {
                _groups.Items.Add(CreateMobileGroup(AndroidDeviceBridge.Platform, "No devices found.", _devices));
                _groups.Items.Add(CreateMobileGroup("iOS", "Disabled during Beta.", null));
                _groups.Items.Add(CreateMobileGroup("Windows", "Disabled during Beta.", null));

                if (_promptDesignerRestart)
                {
                    Application.Instance.AsyncInvoke(async () =>
                    {
                        try
                        {
                            var dlgResult = MessageBox.Show(
                                Application.Instance.MainForm,
                                "It's recommended to restart the mobile designer after loading project assemblies. Continue?",
                                "XenForms",
                                MessageBoxButtons.YesNo,
                                MessageBoxType.Question);

                            _promptDesignerRestart = false;

                            if (dlgResult == DialogResult.Yes)
                            {
                                foreach (var device in _devices)
                                {
                                    await _bridge.StopDesignerAsync(device);
                                    await _bridge.LaunchDesignerAsync(device);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ToolboxApp.Log.Error(e, "Error restarting designer app.");
                        }
                    });
                }
            }
            else
            {
                var help = _bridge.AppDataAdbLocationExists 
                    ? "There was an error communicating with the Android Device Bridge. Please ensure that the Android SDK is installed. Check the log for more information." 
                    : "The Android SDK was not found on your system.\nVerify the Android SDK path through the settings dialog.\n\nYou will have to manually install the designer APK to the device, until this is resolved.\nVisit https://developer.android.com/ to download the Android SDK.";

                _groups.Items.Add(new EmptyDeviceItemView(help));
            }
        }


        private Control CreateMobileGroup(string platform, string noDevicesMessage, MobileDevice[] devices)
        {
            var stack = new StackLayout
            {
                Orientation = Orientation.Vertical,
                Spacing = 10,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            var platformLabel = new Label
            {
                Text = platform,
                Font = new Font(SystemFont.Default, 30f),
                TextColor = new Color(SystemColors.ControlText, .8f),
            };

            var refreshBtn = new ImageButton
            {
                Size = new Size(30,30),
                Image = AppImages.Refresh
            };

            refreshBtn.Click += (sender, args) =>
            {
                if (platform == AndroidDeviceBridge.Platform)
                {
                    CreateGroups();
                }
            };

            var header = new StackLayout
            {
                Spacing = 10,
                VerticalContentAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal
            };

            header.Items.Add(platformLabel);

            if (devices != null)
            {
                header.Items.Add(refreshBtn);
            }

            stack.Items.Add(header);

            if (devices == null || !devices.Any())
            {
                stack.Items.Add(new EmptyDeviceItemView(noDevicesMessage));
            }
            else
            {
                foreach (var device in devices)
                {
                    var item = new DeviceItemView(_bridge, device);
                    stack.Items.Add(item);
                }
            }

            return stack;
        }


        private bool Initialize()
        {
            try
            {
                var adbLocation = ToolboxApp.Settings.GetString(UserSettingKeys.Builtin.AdbLocation);

                if (_bridge == null)
                {
                    _bridge = new AndroidDeviceBridge(adbLocation);
                }

                if (!_bridge.AppDataAdbLocationExists)
                {
                    ToolboxApp.Log.Warn("The Android SDK was not found.");
                    return false;
                }

                ToolboxApp.Log.Info($"Starting the ADB bridge. User defined adb location: {adbLocation ?? "adb.exe location not set."}");
                _devices = _bridge.Start();
                ToolboxApp.Log.Info("ADB bridge started.");

                return true;
            }
            catch (Exception ex)
            {
                ToolboxApp.Log.Error(ex, $"Error querying android devices in {nameof(DeviceManagerView)}.");
                return false;
            }
        }
    }
}
