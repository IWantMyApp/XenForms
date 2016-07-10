using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer.Generators;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions.GetProperties
{
    [TestFixture]
    public class TestGetXamarinStructs : TestBaseReaction
    {
        [Test]
        public void Should_return_LayoutOption_property_and_possible_values()
        {
            Tr.SetTypes(typeof(LayoutOptions));
            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());

            var label = new Label();
            var page = new ContentPage { Content = label };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new [] {"HorizontalOptions"};
                req.WidgetId = label.Id.ToString();
            });

            XamarinFormsReaction.Register<GetObjectRequest, GetObjectReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<ObjectResponse>();
            var p = res.Property;

            Assert.AreEqual(p.PropertyName, "HorizontalOptions");
            Assert.IsTrue(p.XenType.Descriptor.HasFlag(XenPropertyDescriptors.ValueType | XenPropertyDescriptors.Literals));
            Assert.AreEqual(8, p.XenType.PossibleValues.Length);
            Assert.IsAssignableFrom<XenProperty[]>(p.Value);

            var alignmentProp = (p.Value as XenProperty[])?[0];
            Assert.AreEqual(alignmentProp?.Value, "Fill");
            CollectionAssert.IsNotEmpty(alignmentProp?.XenType.PossibleValues);
        }


        [Test]
        public void Should_return_public_properties_of_Bounds_type()
        {
            Tr.SetTypes(typeof(Rectangle));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new []{"Bounds"};
                req.WidgetId = label.Id.ToString();
            });

            XamarinFormsReaction.Register<GetObjectRequest, GetObjectReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<ObjectResponse>();

            var prop = res.Property;
            Assert.AreEqual(prop.PropertyName, "Bounds");

            Assert.IsAssignableFrom<XenProperty[]>(prop.Value);

            var boundProps = (XenProperty[]) prop.Value;
            var xProp = boundProps.FirstOrDefault(b => b.PropertyName == "X");

            Assert.IsNotNull(xProp);
            Assert.AreEqual(xProp.XenType.FullName, typeof (double).FullName);
        }


        [Test]
        public void Private_setters_should_return_struct_as_readonly()
        {
            Tr.SetTypes(typeof(Rectangle));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetObjectRequest>(req =>
            {
                req.Path = new []{"Bounds"};
                req.WidgetId = label.Id.ToString();
            });

            XamarinFormsReaction.Register<GetObjectRequest, GetObjectReaction<VisualElement>>(page);
            Reaction.Execute(ctx);
            var res = ctx.Get<ObjectResponse>();

            var prop = res.Property;
            Assert.AreEqual(prop.PropertyName, "Bounds");

            Assert.IsFalse(prop.CanWrite);
            Assert.IsTrue(prop.XenType.Descriptor.HasFlag(XenPropertyDescriptors.ValueType));
        }


        [Test]
        public void Color_test()
        {
            Tr.SetTypes(typeof(Color));

            var label = new Label
            {
                BackgroundColor = Color.Red
            };

            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(req =>
            {
                req.WidgetId = label.Id.ToString();
                req.IncludeValues = true;
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var res = ctx.Get<GetWidgetPropertiesResponse>();
            var p = res.Properties.First(f => f.PropertyName == "BackgroundColor");
        }
    }
}
