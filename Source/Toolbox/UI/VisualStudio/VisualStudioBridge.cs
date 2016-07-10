#if !XFDESIGNER

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

// ReSharper disable InconsistentNaming
namespace XenForms.Toolbox.UI.VisualStudio
{
    public enum VisualStudioVersion
    {
        VS2015 = 140,
        VS2013 = 120,
        VS2012 = 110,
        VS2010 = 100,
        VS2008 = 90,
        VS2005 = 80,
        VSNet2003 = 71,
        VSNet2002 = 70,
        Other = 0
    }

    public static class VisualStudioBridge
    {
        public const string Exe = "devenv.exe";


        private class LatestVersionComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return (int)y - (int)x;
            }
        }


        public static bool IsInstalled(VisualStudioVersion version)
        {
            var path = GetInstallationFolder(version);
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                var cmd = Path.Combine(path, Exe);
                return File.Exists(cmd);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool OpenFile(string fileName)
        {
            var versions = Enum.GetValues(typeof (VisualStudioVersion));
            Array.Sort(versions, new LatestVersionComparer());

            foreach (var ev in versions)
            {
                var current = (VisualStudioVersion) ev;

                if (IsInstalled(current))
                {
                    return OpenFile(current, fileName);
                }
            }

            return false;
        }


        public static bool OpenFile(VisualStudioVersion version, string fileName)
        {
            if (!IsInstalled(version)) return false;
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            var path = GetInstallationFolder(version);
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                var cmd = Path.Combine(path, Exe);
                Process.Start(cmd, $"/Edit \"{fileName}\"");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static string GetInstallationFolder(VisualStudioVersion version)
        {
            try
            {
                var registryKeyString = string.Format(@"SOFTWARE{0}Microsoft\VisualStudio\{1}",
                    Environment.Is64BitProcess ? @"\Wow6432Node\" : "\\", GetVersionNumber(version));

                using (var localMachineKey = Registry.LocalMachine.OpenSubKey(registryKeyString))
                {
                    return localMachineKey?.GetValue("InstallDir") as string;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        private static string GetVersionNumber(VisualStudioVersion version)
        {
            if (version == VisualStudioVersion.Other) throw new InvalidOperationException("Version not supported.");
            return $"{(int) version/10:f1}";
        }
    }
}

#endif