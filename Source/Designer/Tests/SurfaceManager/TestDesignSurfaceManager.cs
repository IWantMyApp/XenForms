using System;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.Tests.SurfaceManager
{
    [TestFixture]
    public class TestDesignSurfaceManager
    {
        [Test]
        public void ContentView_HasContentProperty()
        {
            var cv = new ContentView();
            var page = new ContentPage { Content = cv };
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var pair = mgr[cv.Id.ToString()];
            Assert.IsTrue(pair.XenWidget.HasContentProperty);
            Assert.IsTrue(pair.XenWidget.IsContentPropertyViewType);
            Assert.AreEqual(pair.XenWidget.ContentPropertyTypeName, typeof(View).FullName);
        }


        [Test]
        public void StackLayout_IsLayout_no_ContentProperty()
        {
            var stack = new StackLayout();
            var page = new ContentPage { Content = stack };
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var pair = mgr[stack.Id.ToString()];
            Assert.AreEqual("System.Collections.IEnumerable<Xamarin.Forms.View>", pair.XenWidget.ContentPropertyTypeName);
            Assert.IsTrue(pair.XenWidget.IsLayout);
            Assert.IsTrue(pair.XenWidget.IsContentPropertyViewType);
            Assert.IsTrue(pair.XenWidget.HasContentProperty);
        }


        [Test]
        public void String_Content_isnt_Layout()
        {
            var label = new Label { Text = "value" };
            var page = new ContentPage { Content = label };
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var pair = mgr[label.Id.ToString()];
            Assert.IsFalse(pair.XenWidget.IsLayout);
            Assert.IsTrue(pair.XenWidget.HasContentProperty);
            Assert.IsFalse(pair.XenWidget.IsContentPropertyViewType);
            Assert.AreEqual(typeof (string).FullName, pair.XenWidget.ContentPropertyTypeName);
        }


        [Test]
        public void Button_has_no_children_and_no_ContentProperty()
        {
            var btn = new Button { Text = "value" };
            var page = new ContentPage { Content = btn };
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var pair = mgr[btn.Id.ToString()];
            Assert.IsFalse(pair.XenWidget.IsLayout);
            Assert.IsFalse(pair.XenWidget.HasContentProperty);
            Assert.IsFalse(pair.XenWidget.IsContentPropertyViewType);
            Assert.AreEqual(null, pair.XenWidget.ContentPropertyTypeName);
        }


        [Test]
        public void When_setting_design_surface_Root_is_set_and_returned()
        {
            var mgr = new XamarinDesignSurfaceManager();
            var page = new ContentPage();

            var root = mgr.SetDesignSurface(page);

            Assert.AreEqual(root, mgr.Root);
        }


        [Test]
        public void When_setting_design_surface_the_page_becomes_the_root_widget()
        {
            var mgr = new XamarinDesignSurfaceManager();
            var page = new ContentPage();

            var root = mgr.SetDesignSurface(page);

            Assert.AreEqual(page.Id.ToString(), root.Id, "Ids aren't equal.");
        }


        [Test]
        public void When_setting_design_surface_the_xenwidget_type_properties_are_set()
        {
            const string title = "Some title.";
            var mgr = new XamarinDesignSurfaceManager();

            var page = new ContentPage
            {
                Title = title
            };

            var root = mgr.SetDesignSurface(page);

            Assert.AreEqual(root.Id, page.Id.ToString(), "Ids aren't equal.");
            Assert.AreEqual(root.FullTypeName, page.GetType().FullName, "Full type names aren't equal.");
            Assert.AreEqual(root.Type, page.GetType().Name, "Short type names aren't equal.");
            Assert.AreEqual(root.Name, title, "The titles aren't equal.");
        }


        [Test]
        public void Can_lookup_xenwidget_by_id()
        {
            var label = new Label {Text = "textvalue"};
            var page = new ContentPage
            {
                Content = label
            };

            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var pagePair = mgr[page.Id.ToString()];
            var labelPair = mgr[label.Id.ToString()];

            Assert.AreEqual(root, pagePair.XenWidget, "The root and page aren't equal.");
            Assert.AreEqual(pagePair.VisualElement, page, "The page wasn't returned.");
            Assert.AreEqual(labelPair.VisualElement, label, "The label wasn't returned.");
            Assert.AreEqual(labelPair.XenWidget.Id, label.Id.ToString(), "The label Ids aren't equal.");
        }


        [Test]
        public void Can_lookup_xenwidget_by_guid()
        {
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();

            mgr.SetDesignSurface(page);
            var match = mgr[page.Id];

            Assert.IsNotNull(match, "Match was null.");
            Assert.AreEqual(match.XenWidget.Id, page.Id.ToString());
        }


        [Test]
        public void When_widget_isnt_found_during_lookup_return_null()
        {
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();
            mgr.SetDesignSurface(page);

            var pair = mgr["12345"];
            Assert.IsNull(pair);
        }


        [Test]
        public void Should_return_null_on_invalid_input()
        {
            var traverser = new XamarinDesignSurfaceManager();

            // Act
            var nullArg = traverser[null];
            Assert.IsNull(nullArg, "Null returns null.");

            var empty = traverser[""];
            Assert.IsNull(empty, "Empty returns null.");

            var whitespace = traverser[" "];
            Assert.IsNull(whitespace, "Whitespace returns null.");
        }


        [Test]
        [Description("The toolbox should prevent this action from happening.")]
        public void Removing_root_xenwidget()
        {
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            Assert.Throws<InvalidOperationException>(() => mgr.Remove(root));
        }


        [Test]
        public void Should_remove_only_design_surface_child_by_using_its_xenwidget()
        {
            var label = new Label {Text = "value"};
            var page = new ContentPage {Content = label};
            var mgr = new XamarinDesignSurfaceManager();
            var root = mgr.SetDesignSurface(page);

            var labelPair = mgr[label.Id.ToString()];
            var removed = mgr.Remove(labelPair.XenWidget);

            Assert.IsTrue(removed, "Removed wasn't true.");
            Assert.IsEmpty(mgr.Root.Children, "mgr.Root.Children should be empty.");
            Assert.IsEmpty(root.Children, "root.Children should be empty.");
            Assert.IsNull(page.Content, "Page content should be null.");
        }


        [Test]
        public void Should_remove_child_of_stacklayout_by_using_its_xenwidget()
        {
            var child = new Entry {Placeholder = "text"};
            var layout = new StackLayout();
            layout.Children.Add(child);
            var page = new ContentPage {Content = layout};

            var mgr = new XamarinDesignSurfaceManager();
            mgr.SetDesignSurface(page);

            var childPair = mgr[child.Id.ToString()];
            var layoutPair = mgr[layout.Id.ToString()];

            Assert.IsNotEmpty(layoutPair.XenWidget.Children, "The stacklayouts xenwidget should have children.");

            var removed = mgr.Remove(childPair.XenWidget);

            Assert.IsTrue(removed, "Removed wasn't true.");
            Assert.IsEmpty(layout.Children, "The stacklayout has children.");
            Assert.IsEmpty(layoutPair.XenWidget.Children, "The stacklayout has children.");
        }


        [Test]
        public void Calling_Contains_should_return_true_when_xenwidget_is_a_descendant_of_root()
        {
            var label = new Label();
            var child = new Entry { Placeholder = "text" };
            var layout = new StackLayout();
            layout.Children.Add(child);
            var page = new ContentPage { Content = layout };

            var mgr = new XamarinDesignSurfaceManager();
            mgr.SetDesignSurface(page);

            var childPair = mgr[child.Id.ToString()];
            var containsWidget = mgr.Contains(childPair.XenWidget);
            var containsVisual = mgr.Contains(childPair.VisualElement);
            var containsLabel = mgr.Contains(label);

            Assert.IsTrue(containsWidget, "XenWidget not found.");
            Assert.IsTrue(containsVisual, "VisualElement not found.");
            Assert.IsFalse(containsLabel, "Label was not added to the page.");
        }


        [Test]
        public void SetParent_page_is_parent_and_label_is_child()
        {
            var label = new Label {Text = "value"};
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();

            mgr.SetDesignSurface(page);

            var parented = mgr.SetParent(label, page);
            Assert.IsTrue(parented, "SetParent returned false.");
            Assert.AreEqual(1, mgr.Root.Children.Count, "Expected one child.");
            Assert.AreEqual(label.Id.ToString(), mgr.Root.Children[0].Id, "The label widget wasn't added to the children collection.");
        }


        [Test]
        public void SetParent_stacklayout_is_parent_and_entry_is_child()
        {
            var child = new Entry();
            var layout = new StackLayout();
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();

            page.Content = layout;
            mgr.SetDesignSurface(page);

            var parented = mgr.SetParent(child, layout);

            Assert.IsTrue(parented, "Parented returned false.");

            var layoutWidget = mgr[layout.Id.ToString()];
            Assert.IsNotEmpty(layoutWidget.XenWidget.Children, "XenWidget did not have any children.");
            Assert.IsNotEmpty(layout.Children, "StackLayout view did not contain children.");
        }


        [Test]
        public void SetParent_entry_is_parent_and_label_is_child()
        {
            var child = new Label();
            var entry = new Entry();
            var page = new ContentPage();
            var mgr = new XamarinDesignSurfaceManager();

            page.Content = entry;
            mgr.SetDesignSurface(page);

            var parented = mgr.SetParent(child, entry);
            Assert.IsFalse(parented, "Parented should be false.");
            Assert.AreEqual(mgr.Root.Children[0].Id, entry.Id.ToString(), "The content view has changed.");
        }


        [Test]
        public void SetParent_ContentView_is_parent_and_label_is_child()
        {
            var child = new Label();
            var content = new ContentView();
            var page = new ContentPage {Content = content};
            var mgr = new XamarinDesignSurfaceManager();

            mgr.SetDesignSurface(page);
            var parented = mgr.SetParent(child, content);

            Assert.IsTrue(parented, "Parented should be true.");

            var contentPair = mgr[content.Id.ToString()];
            Assert.IsNotEmpty(contentPair.XenWidget.Children, "Content children should have an element.");
            Assert.AreEqual(contentPair.XenWidget.Children[0].Id, child.Id.ToString(), "The wrong child was attached.");
        }


        [Test]
        public void SetParent_ScrollView_is_parent_and_entry_is_child()
        {
            var child = new Label();
            var content = new ScrollView();
            var page = new ContentPage { Content = content };
            var mgr = new XamarinDesignSurfaceManager();

            mgr.SetDesignSurface(page);
            var parented = mgr.SetParent(child, content);

            Assert.IsTrue(parented, "Parented should be true.");

            var contentPair = mgr[content.Id.ToString()];
            Assert.IsNotEmpty(contentPair.XenWidget.Children, "Content children should have an element.");
            Assert.AreEqual(contentPair.XenWidget.Children[0].Id, child.Id.ToString(), "The wrong child was attached.");
        }
    }
}
