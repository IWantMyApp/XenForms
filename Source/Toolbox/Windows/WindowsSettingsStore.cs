using System;
using Microsoft.Win32;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;

namespace XenForms.Windows
{
    public class WindowsSettingsStore : ISettingsStore
    {
        public const string Path = "Software\\XenForms";
        private readonly ToolboxLogging _log;


        public WindowsSettingsStore(ToolboxLogging log)
        {
            _log = log;
        }


        public RegistryKey GetStore()
        {
            try
            {
                using (var key = Registry.CurrentUser)
                {
                    key.CreateSubKey(Path);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to create XenForms Windows registry key. User settings will not be used.");
                return null;
            }

            try
            {
                var root = Registry.CurrentUser.OpenSubKey(Path, true);

                if (root == null)
                {
                    _log.Error("XenForms registry key not found. User settings will not be used.");
                }

                return root;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to open XenForms registry key. User settings will not be used..");
                return null;
            }
        }


        public bool Set(string key, string value)
        {
            return DoSet(key, value ?? string.Empty, RegistryValueKind.String);
        }


        public bool Set(string key, bool value)
        {
            return DoSet(key, value, RegistryValueKind.DWord);
        }


        public string GetString(string key)
        {
            var val = DoGet(key);
            return val?.ToString();
        }


        public bool? GetBool(string key)
        {
            var raw = DoGet(key);
            if (raw == null) return null;

            return (int) raw == 1;
        }


        private bool DoSet(string key, object value, RegistryValueKind kind)
        {
            var store = GetStore();
            if (store == null) return false;

            try
            {
                store.SetValue(key, value, kind);
                store.Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to set registry value for key: {key}.");
                return false;
            }

            return true;
        }


        private object DoGet(string key)
        {
            var store = GetStore();
            if (store == null) return null;

            try
            {
                var val = store.GetValue(key);
                store.Close();
                return val;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to get registry value for key: {key}.");
                return null;
            }
        }
    }
}