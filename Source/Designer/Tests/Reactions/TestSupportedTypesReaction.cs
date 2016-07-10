using NUnit.Framework;
using XenForms.Core.Designer;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Designer.Tests.Reactions
{
    [TestFixture]
    public class TestSupportedTypesReaction
    {
        private TypeRegistrar Tr => TypeRegistrar.Instance;


        [Test]
        public void Register_supported_types_when_message_is_received()
        {
            var ctx = new XenMessageContext();
            ctx.SetRequest<SupportedTypesRequest>(r =>
            {
                r.Types = new[]
                {
                    new XenType
                    {
                        Descriptor = XenPropertyDescriptors.None,
                        FullName = "String"
                    },
                    new XenType
                    {
                        Descriptor = XenPropertyDescriptors.None,
                        FullName = "Integer"
                    }
                };
            });

            Reaction.Register<SupportedTypesRequest, SupportedTypesReaction>();
            Reaction.Execute(ctx);
            Assert.IsTrue(Tr.Types.Count == 2);
        }
    }
}
