using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    public class FakeNoResponseReaction : Reaction
    {
        public bool Completed { get; set; }
        public XenMessageContext Context { get; set; }

        protected override void OnExecute(XenMessageContext ctx)
        {
            Completed = true;
            Context = ctx;
        }
    }
}