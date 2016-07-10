using System;
using System.Text.RegularExpressions;

namespace XenForms.Core.Networking
{
    public class ServiceEndpoint
    {

#if DEBUG
        // when the debugger is attached, we want a higher value so we're not disconnected.
        public static readonly TimeSpan PingWaitTime = TimeSpan.FromMinutes(25);
#else
        public static readonly TimeSpan PingWaitTime = TimeSpan.FromMinutes(5);
#endif

        public const short Port = 9074;
        public const string DesignerPath = "/";
        public const string ToolboxPath = "/toolbox";
        private static readonly Regex PortRegex = new Regex(@"^\d+$");


        public static bool IsValidDnsLabel(string host)
        {
            return Uri.CheckHostName(host) != UriHostNameType.Unknown;
        }

        public static bool IsValidPort(string port)
        {
            if (string.IsNullOrWhiteSpace(port)) return false;

            if (PortRegex.IsMatch(port))
            {
                short result;
                var parsed = short.TryParse(port, out result);
                return parsed && IsValidPort(result);
            }

            return false;
        }


        public static bool IsValidPort(short port)
        {
            return port > 1023;
        }


        public static string FormatAddress(string host, short port, string path = null)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }

            if (!IsValidDnsLabel(host))
            {
                return null;
            }

            if (!IsValidPort(port))
            {
                return null;
            }

            var segment = "/";

            if (path != null)
            {
                segment = path;
            }

            return $"ws://{host}:{port}{segment}";
        }
    }
}
