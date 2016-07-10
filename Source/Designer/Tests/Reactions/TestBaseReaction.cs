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
using XenForms.Designer.Tests.Reactions.SetProperties;
using XenForms.Designer.XamarinForms.UI;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.Reactions
{
    public class TestBaseReaction
    {
        protected static TypeRegistrar Tr => TypeRegistrar.Instance;
        protected DescriptorRegistrar Dr { get; set; }

        [SetUp]
        public void BeforeTest()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IDesignThread>().To<FakeDesignThread>();
            kernel.Bind<DesignSurfaceManager<VisualElement>>().To<XamarinDesignSurfaceManager>();
            kernel.Bind<IXenCodeLoader>().To<FakeXenCodeLoader>();
            kernel.Bind<ITypeFinder>().To<TypeFinder>();

            Dr = DescriptorRegistrar.Create(new TypeFinder());
            Dr.Reset();

            Reaction.Reset();
            Reaction.GetServices = obj => { kernel.Inject(obj); };
        }


        public PageWithPrimitives SetPrimitiveProperty(string path, string value)
        {
            var page = new PageWithPrimitives
            {
                Content = new Label()
            };

            XamarinFormsReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            var ctx = new XenMessageContext();

            ctx.SetRequest<SetPropertyRequest>(req =>
            {
                req.WidgetId = page.Id.ToString();
                req.Path = new[] { path };
                req.Value = value;
            });

            Reaction.Execute(ctx);

            return page;
        }
    }
}
