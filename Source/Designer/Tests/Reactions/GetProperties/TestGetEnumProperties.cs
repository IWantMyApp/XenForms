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
    public class TestGetEnumProperties : TestBaseReaction
    {
        [Test]
        public void Should_return_property_types_and_names()
        {
            Tr.SetTypes(typeof (Enum));

            var label = new Label();
            var page = new ContentPage
            {
                Content = label
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = label.Id.ToString();
            });

            Dr.Add(typeof (Enum), new EnumGenerator());
            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("HorizontalTextAlignment"));
            Assert.IsNotNull(property, "Property not found.");

            Assert.AreEqual(property.Value, TextAlignment.Start.ToString());
            Assert.AreEqual(property.XenType.Descriptor, XenPropertyDescriptors.Literals);

            // if this occurs, types will not be selected correctly by the toolbox
            Assert.IsFalse(property.XenType.Descriptor.HasFlag(XenPropertyDescriptors.ValueType));
            CollectionAssert.IsNotEmpty(property.XenType.PossibleValues);
        }
    }
}