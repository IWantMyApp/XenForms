using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.EmptyFakes
{
    /// <summary>
    /// Used for tracking the raw data sent through the designer action.
    /// </summary>
    sealed class FakeTrackRequestReaction : Reaction
    {
        private string RawRequest { get; }


        public FakeTrackRequestReaction(string rawRequest)
        {
            RawRequest = rawRequest;
        }


        protected override void OnExecute(XenMessageContext context)
        {
            context.Message = RawRequest;
        }
    }
}