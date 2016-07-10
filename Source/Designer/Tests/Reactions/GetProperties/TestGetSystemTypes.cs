using System;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer.Generators;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Designer.Tests.Reactions.SetProperties;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions.GetProperties
{
    public class TestGetSystemTypes : TestBaseReaction
    {
        [Test]
        public void Should_return_Guid_as_string()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof (Guid));
        
            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = label.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var prop = res.Properties[0];

            Assert.IsFalse(prop.XenType.IsNullable);
            Assert.IsNotNull(prop.Value);
        }


        [Test]
        public void Should_get_nint()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(Guid), typeof(int?));

            var page = new PageWithPrimitives();
            var ctx = new XenMessageContext();

            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var type = res.Properties[0].XenType;

            Assert.IsTrue(type.IsNullable);
            Assert.AreEqual("Int32?", type.ShortName);
        }


        [Test]
        public void Should_get_nchar()
        {
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Tr.SetTypes(typeof(Guid), typeof(char?));

            var page = new PageWithPrimitives();
            var ctx = new XenMessageContext();

            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var type = res.Properties[0].XenType;

            Assert.IsTrue(type.IsNullable);
            Assert.AreEqual("Char?", type.ShortName);
        }
    }
}
