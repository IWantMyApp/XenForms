using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions.GetProperties
{
    [TestFixture]
    public class TestGetNumericProperties : TestBaseReaction
    {
        [Test]
        public void Should_return_property_types_and_names()
        {
            const string requestedType = "System.Guid";

            Tr.Types = new HashSet<XenType>(new[]
            {
                new XenType
                {
                    FullName = requestedType
                }
            });

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

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("Id"));

            Assert.IsNotNull(property, "Id property should have been found.");
            Assert.AreEqual(property.XenType.FullName, typeof(Guid).FullName);
            Assert.IsNotNull(property.Value, "Value should have been set.");
        }
    }
}
