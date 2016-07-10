using System.Linq;
using Ninject;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.Tests.Reactions
{
    [TestFixture]
    public class TestDesignerReactions
    {
        [SetUp]
        public void BeforeTest()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IDesignThread>().To<XamarinDesignThread>();
            kernel.Bind<DesignSurfaceManager<VisualElement>>().To<XamarinDesignSurfaceManager>();

            Reaction.Reset();
            Reaction.GetServices = obj => { kernel.Inject(obj); };
        }


        [Test]
        public void Only_attach_method_can_add_widgets_to_hierarchy()
        {
            var traverser = new XamarinDesignSurfaceManager();

            var layout = new StackLayout
            {
                Children =
                {
                    new Label {Text = "For good measure..."}
                }
            };

            var page = new ContentPage
            {
                Title = "Testing that a newly added XenWidget can be found in the hierarchy",
                Content = layout
            };

            var addLater = new Label { Text = "Added after hierarchy built." };

            // Act
            var root = traverser.SetDesignSurface(page);
            var firstWidgets = root.GetNodeAndDescendants().ToArray();

            Assert.IsNull(firstWidgets.FirstOrDefault(w => w.Id == addLater.Id.ToString()), "Recurisve search found label.");

            // Act
            layout.Children.Add(addLater);

            // The root element is fixed. Its the page.
            var secondWidgets = root.GetNodeAndDescendants().ToArray();
            Assert.IsNull(secondWidgets.FirstOrDefault(w => w.Id == addLater.Id.ToString()), "Recurisve search found label.");
        }
    }
}
