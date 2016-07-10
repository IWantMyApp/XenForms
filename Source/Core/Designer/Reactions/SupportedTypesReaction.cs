using System.Collections.Generic;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// All types known by the toolbox are contained in a <see cref="SupportedTypesRequest"/>.
    /// </summary>
    public class SupportedTypesReaction : Reaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<SupportedTypesRequest>();
            if (request == null) return;

            TypeRegistrar.Instance.Types = new HashSet<XenType>(request.Types);
        }
    }
}