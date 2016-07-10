using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    sealed class FakeCompletedReaction : Reaction
    {
        protected override void OnExecute(XenMessageContext context)
        {
            context.Response = new FakeCompletedResponse
            {
                Action = nameof(FakeCompletedResponse),
            };
        }
    }
}