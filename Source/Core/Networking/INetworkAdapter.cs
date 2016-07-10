namespace XenForms.Core.Networking
{
    /// <summary>
    /// .NET PCL's do not have the ability to query a network adapter for an IP address.
    /// XenForm designers and toolboxes should implement this interface.
    /// </summary>
    public interface INetworkAdapter
    {
        string GetIp();
        bool IsResponsive(string address);
    }
}
