namespace XenForms.Core.Messages
{
    /// <summary>
    /// Sent before the design server stops responding to requests.
    /// When this is sent, the toolbox is informed that the design server executed it's shutdown
    /// routine without error.
    /// </summary>
    public class DesignServerStoppedOk : XenMessage {}
}
