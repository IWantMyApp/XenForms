using System;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer.Generators;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions.Constructors
{
    [TestFixture]
    public class TestGetConstructorsReaction : TestBaseReaction
    {
        [Test]
        public void Should_respond_with_GridLength_type_with_two_constructors()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(GridLength));

            var page = new ContentPage();
            var ctx = new XenMessageContext();

            var typeName = typeof (GridLength).FullName;

            ctx.SetRequest<GetConstructorsRequest>(req =>
            {
                req.TypeName = typeName;
            });

            XamarinFormsReaction.Register<GetConstructorsRequest, GetConstructorsReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetConstructorsResponse>();

            Assert.IsNotNull(res.Type);
            Assert.AreEqual(typeName, res.Type.FullName);
            Assert.AreEqual(2, res.Type.Constructors.Length);

            foreach (var ctor in res.Type.Constructors)
            {
                foreach (var p in ctor.Parameters)
                {
                    Assert.IsNotEmpty(p.XenType.PossibleValues);
                    Assert.AreEqual(p.TypeName, p.XenType.FullName);
                }
            }
        }
    }
}
