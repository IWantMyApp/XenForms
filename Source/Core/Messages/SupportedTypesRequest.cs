using XenForms.Core.Designer;
using XenForms.Core.Widgets;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// Sent immediately after a toolbox has connected to the design server.
    /// The <see cref="Types"/> collection contains all known types that can be created, updated, read, or deleted from 
    /// the toolbox.
    /// </summary>
    public class SupportedTypesRequest : Request
    {
        public XenType[] Types { get; set; }
    }
}
