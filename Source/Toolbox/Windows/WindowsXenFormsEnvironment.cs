using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;
using XenForms.Core;

namespace XenForms.Windows
{
    public class WindowsXenFormsEnvironment : XenFormsEnvironment
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };


        public override string GetOsInformation()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GetWindowsVersion());
            sb.AppendLine(Environment.OSVersion.VersionString);
            sb.AppendLine($"64bit OS: {Environment.Is64BitOperatingSystem}");
            return sb.ToString();
        }


        public override string GetMemoryUsage()
        {
            /*
            PrivateMemorySize
            The number of bytes that the associated process has allocated that cannot be shared with other processes.
            PeakVirtualMemorySize
            The maximum amount of virtual memory that the process has requested.
            PeakPagedMemorySize
            The maximum amount of memory that the associated process has allocated that could be written to the virtual paging file.
            PagedSystemMemorySize
            The amount of memory that the system has allocated on behalf of the associated process that can be written to the virtual memory paging file.
            PagedMemorySize
            The amount of memory that the associated process has allocated that can be written to the virtual memory paging file.
            NonpagedSystemMemorySize
            The amount of memory that the system has allocated on behalf of the associated process that cannot be written to the virtual memory paging file.
            */

            var p = Process.GetCurrentProcess();
            var sb = new StringBuilder();

            sb.AppendLine($"Private memory size: {Format(p.PrivateMemorySize64)}");
            sb.AppendLine($"Working Set size: {Format(p.WorkingSet64)}");
            sb.AppendLine($"Peak virtual memory size: {Format(p.PeakVirtualMemorySize64)}");
            sb.AppendLine($"Peak paged memory size: {Format(p.PeakPagedMemorySize64)}");
            sb.AppendLine($"Paged system memory size: {Format(p.PagedSystemMemorySize64)}");
            sb.AppendLine($"Paged memory size: {Format(p.PagedMemorySize64)}");
            sb.AppendLine($"Nonpaged system memory size: {Format(p.NonpagedSystemMemorySize64)}");

            return sb.ToString();
        }


        private static string GetWindowsVersion()
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return (string) key?.GetValue("ProductName");
            }
            catch (Exception)
            {
                return "Unable to query registry for Windows product name.";
            }
        }


        private static string Format(long value)
        {
            if (value < 0) { return "-" + Format(-value); }
            if (value == 0) { return "0.0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return $"{adjustedSize:n1} {SizeSuffixes[mag]}";
        }
    }
}