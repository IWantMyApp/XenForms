using XenForms.Core.Designer;
using XenForms.Core.Designer.Reactions;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// Toolbox requests require a response. If a <see cref="Reaction"/> did not create a specific
    /// response for the request, an object of this request is sent.
    /// </summary>
    public class OkResponse : Response
    {
        /// <summary>
        /// The <see cref="XenMessage.MessageId"/> of the parent request.
        /// </summary>
        public string ReplyingTo { get; set; }
    }
}
