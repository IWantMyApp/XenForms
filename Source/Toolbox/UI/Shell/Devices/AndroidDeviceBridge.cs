using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Shell.Devices
{
    public class AndroidDeviceBridge
    {
        public const string Platform = "Android";
        public const int LaunchTimeout = 10000;
        public const string PackageName = "com.xenforms.designer";
        public const string ApkName = "com.xenforms.designer.apk";

        public string TempPackage => $"/data/local/tmp/{ApkName}";
        public string DesignerLaunchCommand => $"am start -n {PackageName}/{PackageName}.MainActivity";
        public string DesignerStopCommand => $"am force-stop {PackageName}";
        public string DesignerInstalledCommand => $"pm list packages {PackageName}";
        public string ProcessRunningCommand => $"ps | grep {PackageName}";
        public string WiFiAddressCommand => "ifconfig wlan0";
        public string VersionCodeCommand => $"dumpsys package {PackageName} | grep versionCode";

        private readonly Dictionary<string, DeviceData> _devices = new Dictionary<string, DeviceData>();
        private readonly string _customAdbLocation;


        public AndroidDeviceBridge(string adbLocation = null)
        {
            _customAdbLocation = adbLocation;
        }


        public string AppDataAdbLocation
        {
            get
            {
                if (File.Exists(_customAdbLocation))
                {
                    ToolboxApp.Log.Info("Android's adb.exe was found using user setting. Lookup ended.");
                    return _customAdbLocation;
                }

                var regPath = GetAndroidSdkRegistryEntry();
                if (!string.IsNullOrWhiteSpace(regPath))
                {
                    var regLoc = Path.Combine(regPath, "platform-tools\\adb.exe");

                    if (File.Exists(regLoc))
                    {
                        return regLoc;
                    }
                }

                var pX86Adb = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"\Android\android-sdk\platform-tools\adb.exe");

                if (File.Exists(pX86Adb))
                {
                    return pX86Adb;
                }

                var asdk = BuildAdbPath("android-sdk");

                if (File.Exists(asdk))
                {
                    return asdk;
                }

                var sdk = BuildAdbPath("sdk");

                if (File.Exists(sdk))
                {
                    return sdk;
                }

                ToolboxApp.Log.Warn("Android's adb.exe was not found. Designer installs will be manual.");
                return null;
            }
        }


        public bool AppDataAdbLocationExists => File.Exists(AppDataAdbLocation);


        public MobileDevice[] Start()
        {
            AdbServer.Instance.StartServer(AppDataAdbLocation, false);

            _devices.Clear();
            var devices = AdbClient.Instance.GetDevices();
            var result = new List<MobileDevice>();

            foreach (var device in devices)
            {
                var id = Guid.NewGuid().ToString();

                var android = new MobileDevice
                {
                    Model = device.Model,
                    Id = id,
                    Address = GetIpAddress(device.Serial),
                    Status = device.State.ToString(),
                    Product = device.Product
                };

                var count = android.Address.Count(c => c == '.');
                android.IsEmulator = count > 2;

                _devices.Add(id, device);
                result.Add(android);
            }

            return result.ToArray();
        }


        public async Task<bool> IsRecentVersionInstalledAsync(MobileDevice device, int version)
        {
            var data = GetDeviceData(device);
            var installed = await CheckDesignerInstalledAsync(device);

            if (!installed) return false;

            var manager = new PackageManager(data);
            var info = manager.GetVersionInfo(PackageName);

            if (info != null)
            {
                return version == info.VersionCode;
            }

            var legacyCheck = await LegacyCheckVersionInstalledAsync(device, version);
            return legacyCheck;
        }


        public async Task<bool> LegacyCheckVersionInstalledAsync(MobileDevice device, int version)
        {
            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();

            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(VersionCodeCommand, data, recv, CancellationToken.None, LaunchTimeout);

            var result = recv.ToString();

            if (result.Contains($"versionCode={version}"))
            {
                return true;
            }

            return false;
        }


        public async Task InstallDesignerAsync(MobileDevice device, string localApk)
        {
            await Task.Run(() =>
            {
                var data = GetDeviceData(device);

                using (var service = new SyncService(data))
                {
                    using (Stream stream = File.OpenRead(localApk))
                    {
                        service.Push(stream, TempPackage, 777, DateTime.Now, null, CancellationToken.None);
                    }
                }

                var manager = new PackageManager(data);
                manager.InstallRemotePackage(TempPackage, true);
            });
        }


        public async Task<bool> CheckDesignerInstalledAsync(MobileDevice device)
        {
            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();
            
            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(DesignerInstalledCommand, data, recv, CancellationToken.None, LaunchTimeout);

            var result = recv.ToString();

            if (result.Contains($"package:{PackageName}"))
            {
                return true;
            }

            return false;
        }


        public async Task LaunchDesignerAsync(MobileDevice device)
        {
            var running = await IsDesignerRunning(device);

            if (running)
            {
                return;
            }

            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();

            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(DesignerLaunchCommand, data, recv, CancellationToken.None, LaunchTimeout);
        }


        public async Task StopDesignerAsync(MobileDevice device)
        {
            var running = await IsDesignerRunning(device);

            if (!running)
            {
                return;
            }

            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();

            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(DesignerStopCommand, data, recv, CancellationToken.None, LaunchTimeout);
        }


        public async Task<string> GetConnectedDeviceIpAddressAsync(MobileDevice device)
        {
            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();

            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(WiFiAddressCommand, data, recv, CancellationToken.None, LaunchTimeout);

            var output = recv.ToString();

            return ParseIpAddress(output);
        }


        public async Task<bool> IsDesignerRunning(MobileDevice device)
        {
            await CheckDesignerInstalledAsync(device);

            var data = GetDeviceData(device);
            var recv = new ConsoleOutputReceiver();

            await AdbClient
                .Instance
                .ExecuteRemoteCommandAsync(ProcessRunningCommand, data, recv, CancellationToken.None, LaunchTimeout);

            return recv.ToString().Contains(PackageName);
        }


        public async Task<Image> CaptureScreenAsync(MobileDevice device)
        {
            var data = GetDeviceData(device);
            return await AdbClient.Instance.GetFrameBufferAsync(data, CancellationToken.None);
        }


        private DeviceData GetDeviceData(MobileDevice device)
        {
            if (!_devices.ContainsKey(device.Id))
            {
                throw new InvalidOperationException($"{nameof(Start)} must be called, first.");
            }

            return _devices[device.Id];
        }


        private string BuildAdbPath(string folderName)
        {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(local, "Android", folderName, "platform-tools", "adb.exe");
            return path;
        }


        private string GetAndroidSdkRegistryEntry()
        {
            try
            {
                var path = string.Format(@"SOFTWARE{0}Android SDK Tools", 
                    Environment.Is64BitProcess ? @"\Wow6432Node\" : @"\");

                using (var entry = Registry.LocalMachine.OpenSubKey(path))
                {
                    return entry?.GetValue("Path") as string;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        private string GetIpAddress(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial)) return string.Empty;
            if (!serial.Contains(":")) return serial;

            var tmp = serial.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length == 0) return string.Empty;

            return tmp[0];
        }


        private string ParseIpAddress(string output)
        {
            if (string.IsNullOrWhiteSpace(output)) return null;

            var tmp = output.Split(' ');
            if (tmp.Length == 0) return null;

            var index = Array.IndexOf(tmp, "ip") + 1;
            if (index >= tmp.Length) return null;

            var address = tmp[index];
            return address?.Trim();
        }
    }
}
