using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using XenForms.Core.Networking;

namespace XenForms.Core.Platform
{
    public class NetworkAdapter : INetworkAdapter
    {
        public string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("A local IPV4 address was not available for binding.");
        }


        public bool IsResponsive(string address)
        {
            var pingable = false;
            var pinger = new Ping();

            try
            {
                var reply = pinger.Send(address);
                pingable = reply?.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // ignored
            }

            return pingable;
        }
    }
}