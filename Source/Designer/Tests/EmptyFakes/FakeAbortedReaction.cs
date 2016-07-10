using System;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    sealed class FakeAbortedReaction : Reaction
    {
        protected override void OnExecute(XenMessageContext context)
        {
            throw new InvalidOperationException();
        }
    }
}