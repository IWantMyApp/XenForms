using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    public class FakeQueueReaction : Reaction
    {
        public XenMessageContext Context { get; set; }


        protected override void OnExecute(XenMessageContext ctx)
        {
            ctx.SetResponse<FakeQueueResponse>(r =>
            {
                r.Completed = true;
            });

            Context = ctx;
        }
    }
}
