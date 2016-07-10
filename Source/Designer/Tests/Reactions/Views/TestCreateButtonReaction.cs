using Ninject;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Designer;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Platform.Reflection;
using XenForms.Core.Reflection;
using XenForms.Designer.Tests.Reactions.EmptyFakes;
using XenForms.Designer.XamarinForms.UI;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions.Views
{
    [TestFixture]
    public class TestCreateButtonReaction
    {
        [SetUp]
        public void BeforeTest()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IDesignThread>().To<FakeDesignThread>();
            kernel.Bind<DesignSurfaceManager<VisualElement>>().To<XamarinDesignSurfaceManager>();
            kernel.Bind<IXenCodeLoader>().To<FakeXenCodeLoader>();
            kernel.Bind<ITypeFinder>().To<TypeFinder>();

            Reaction.Reset();
            Reaction.GetServices = obj => { kernel.Inject(obj); };
        }


        public void Todo()
        {
            // create a string array of xamarin.forms views and itterate through them.
            // each one should be instantiated using the below
            Assert.Fail();
        }

        [Test]
        public void Attach_button_to_grid_and_not_set_row_and_col()
        {
            var grid = new Grid();
            var page = new ContentPage
            {
                Title = "Testing create button action",
                Content = grid
            };

            var ctx = new XenMessageContext();
            ctx.SetRequest<CreateWidgetRequest>(r =>
            {
                r.ParentId = grid.Id.ToString();
                r.TypeName = "Xamarin.Forms.Button";
            });

            XamarinFormsReaction.Register<CreateWidgetRequest, CreateWidgetReaction>(page);
            Reaction.Execute(ctx);

            var response = ctx.Get<CreateWidgetResponse>();

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.Parent.Type == nameof(Grid), "Expected type to be grid.");
            Assert.IsTrue(response.Widget.Type == nameof(Button), "Expected type to be button.");
            Assert.IsTrue(response.Parent.Children[0].Type == nameof(Button), "Expected child to be button.");
        }
    }
}
