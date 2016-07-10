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
    public class TestGetWidgetPropertiesReaction : TestBaseReaction
    {
        [Test]
        public void Should_return_properties_for_entry()
        {
            Tr.Types = new HashSet<XenType>(new[]
            {
                new XenType
                {
                    FullName = "System.String"
                }
            });

            var entry = new Entry();
            var page = new ContentPage
            {
                Content = entry
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = entry.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            Assert.IsTrue(response.Properties.Any(), "Expected properties.");
        }


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

            var entry = new Entry();
            var page = new ContentPage
            {
                Content = entry
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = entry.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("Id"));

            Assert.IsNotNull(property, "Id property should have been found.");
            Assert.AreEqual(property.XenType.FullName, typeof(Guid).FullName);
            Assert.IsNotNull(property.Value, "Value should have been set.");
        }


        [Test]
        public void Should_return_properties_for_ContentPage()
        {
            Tr.SetTypes(typeof (String));

            var page = new ContentPage
            {
                Title = "Important",
                Content = new Label {  Text = "Does Nothing" }
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = page.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            var property = response?.Properties.FirstOrDefault(p => p.PropertyName.Equals("Title"));

            Assert.IsNotNull(property, "Title property should have been found.");
            Assert.AreEqual(property.XenType.FullName, typeof(String).FullName);
            Assert.IsNotNull(property.Value, "Value should have been set.");
        }


        [Test]
        public void Should_mark_getter_properties_only_as_readonly()
        {
            Tr.Types = new HashSet<XenType>(new[]
            {
                new XenType
                {
                    Descriptor = XenPropertyDescriptors.None,
                    FullName = "System.String",
                },
                new XenType
                {
                    Descriptor = XenPropertyDescriptors.None,
                    FullName = "System.Boolean"
                },
                new XenType
                {
                    Descriptor = XenPropertyDescriptors.None,
                    FullName = "Xamarin.Forms.Rectangle"
                }
            });

            var label = new Label
            {
                Text = "My Label"
            };

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
            Assert.IsTrue(response.Properties.Any(), "Expected properties.");

            var focused = response.Properties.FirstOrDefault(p => p.PropertyName == "IsFocused");
            Assert.IsFalse(focused?.CanWrite, "IsFocus is readonly");
        }


        [Test]
        public void Should_filter_unsupported_types()
        {
            const string requestedType = "System.String";
            Tr.Types = new HashSet<XenType>(new[]
            {
                new XenType
                {
                    Descriptor = XenPropertyDescriptors.None,
                    FullName = requestedType
                }
            });

            var entry = new Entry();
            var page = new ContentPage
            {
                Content = entry
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = entry.Id.ToString();
            });


            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();
            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsNotNull(response.Widget, "Widget should not be null.");

            if (response.Properties != null)
            {
                foreach (var property in response.Properties)
                {
                    if (!property.XenType.FullName.Equals(requestedType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Assert.Fail($"Expected {requestedType}, but {property.XenType.FullName} was returned.");
                    }
                }

                Assert.IsNotNull(response.Properties.First(p => p.PropertyName == "Text"), "The text property was not found.");
            }
        }


        [Test]
        public void Should_not_return_value_for_UserObjects()
        {
            Tr.Types = new HashSet<XenType>(new[]
            {
                new XenType
                {
                    FullName = "System.String",
                },
                new XenType
                {
                    FullName = "System.Boolean"
                },
                new XenType
                {
                    FullName = "Xamarin.Forms.Rectangle"
                },
                new XenType
                {
                    FullName = "Xamarin.Forms.Font"
                }
            });

            var label = new Label
            {
                Text = "My Label"
            };

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
            var pBounds = response.Properties.First(p => p.PropertyName == "Bounds");
            Assert.IsNull(pBounds.Value);
            Assert.IsTrue(pBounds.XenType.Descriptor == XenPropertyDescriptors.ValueType);

            var pFont = response.Properties.First(p => p.PropertyName == "Font");
            Assert.IsNull(pFont.Value);
            Assert.IsTrue(pFont.XenType.Descriptor == XenPropertyDescriptors.ValueType);

            var pLabel = response.Properties.FirstOrDefault(p => p.PropertyName == "Text");
            Assert.IsNotNull(pLabel?.Value);
            Assert.IsTrue(pLabel.XenType.Descriptor != XenPropertyDescriptors.ValueType);
        }
    }
}