using Ninject.Modules;
using Xamarin.Forms;
using XenForms.Core.Designer;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Designer.Workflow;
using XenForms.Core.Networking;
using XenForms.Core.Platform;
using XenForms.Core.Platform.Reflection;
using XenForms.Core.Reflection;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.XamarinForms.Droid
{
    public class DroidModule : NinjectModule
    {
        public override void Load()
        {
            Bind<INetworkAdapter>().To<NetworkAdapter>();
            Bind<IXamlLoader>().To<XamlLoader>();
            Bind<IXenMessageFinder>().To<XenMessageFinder>();
            Bind<DesignSurfaceManager<VisualElement>>().To<XamarinDesignSurfaceManager>();
            Bind<IDesignThread>().To<XamarinDesignThread>();
            Bind<DesignServer>().To<DefaultDesignServer>().InSingletonScope();
            Bind<DesignWorkflow>().To<DefaultDesignWorkflow>().InSingletonScope();
            Bind<IXenCodeLoader>().To<XenCodeLoader>();
            Bind<ITypeFinder>().To<TypeFinder>();
            Bind<IGetDesignerVersion>().To<GetDesignerVersion>();
        }
    }
}