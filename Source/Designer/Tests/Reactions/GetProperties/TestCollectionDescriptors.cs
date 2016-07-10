using System.CodeDom;
using System.Collections.Generic;
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
    public class TestCollectionDescriptors : TestBaseReaction
    {
        private class NullCollectionView : ContentView
        {
            public IEnumerable<object> Enumerable { get; set; }
            public ICollection<string> Collection { get; set; }
            public IList<int> List { get; set; }
        }


        [Test]
        public void Null_collections()
        {
            Tr.SetTypes(typeof(IEnumerable<object>), typeof(ICollection<string>), typeof(IList<int>));

            var view = new NullCollectionView
            {
                Enumerable = null,
                Collection = null,
                List = null
            };

            var page = new ContentPage
            {
                Content = view
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = view.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            foreach (var p in response.Properties)
            {
                var d = p.XenType.Descriptor;

                if (p.PropertyName == "Enumerable")
                {
                    Assert.AreEqual(d, XenPropertyDescriptors.Enumerable);
                }

                if (p.PropertyName == "Collection")
                {
                    Assert.AreEqual(d, XenPropertyDescriptors.Enumerable | XenPropertyDescriptors.Collection);
                }

                if (p.PropertyName == "List")
                {
                    Assert.AreEqual(d, XenPropertyDescriptors.Enumerable 
                        | XenPropertyDescriptors.Collection 
                        | XenPropertyDescriptors.List);
                }

                Assert.IsNull(p.XenType.PossibleValues);
            }   
        }


        [Test]
        public void Empty_collections()
        {
            Tr.SetTypes(typeof(RowDefinitionCollection), typeof(ColumnDefinitionCollection));
        
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(),
                RowDefinitions = new RowDefinitionCollection()
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = grid.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            foreach (var p in response.Properties)
            {
                var pv = p.XenType.PossibleValues;
                CollectionAssert.IsNotEmpty(pv);
                Assert.AreEqual("0", pv[0]);
            }
        }


        [Test]
        public void Enumerable_collection_and_list_interfaces_found()
        {
            Tr.SetTypes(typeof(GridLength), typeof(RowDefinitionCollection),
                typeof(ColumnDefinitionCollection), typeof(RowDefinition), typeof(ColumnDefinition));

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    // 3 columns required for test
                    new ColumnDefinition {Width = new GridLength(50, GridUnitType.Absolute)},
                    new ColumnDefinition {Width = new GridLength(25, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(25, GridUnitType.Auto)},
                },

                RowDefinitions =
                {
                    // 3 rows required for test
                    new RowDefinition {Height = new GridLength(100)},
                    new RowDefinition {Height = new GridLength(100, GridUnitType.Star)},
                    new RowDefinition {Height = new GridLength(100, GridUnitType.Star)},
                }
            };

            var page = new ContentPage
            {
                Content = grid
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<GetWidgetPropertiesRequest>(r =>
            {
                r.WidgetId = grid.Id.ToString();
            });

            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<GetWidgetPropertiesResponse>();

            foreach (var p in response.Properties)
            {
                var pv = p.XenType.PossibleValues;
                var d = p.XenType.Descriptor;

                CollectionAssert.IsNotEmpty(pv);
                Assert.AreEqual(1, pv.Length);
                Assert.AreEqual("3", pv[0]);

                Assert.IsNull(p.Value);

                Assert.IsTrue
                (
                    d.HasFlag(XenPropertyDescriptors.Enumerable) &&
                    d.HasFlag(XenPropertyDescriptors.Collection) &&
                    d.HasFlag(XenPropertyDescriptors.List)
                );
            }
        }
    }
}
