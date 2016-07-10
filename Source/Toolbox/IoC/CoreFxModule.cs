using Ninject.Modules;
using XenForms.Core.Diagnostics;
using XenForms.Core.FileSystem;
using XenForms.Core.Networking;
using XenForms.Core.Platform;
using XenForms.Core.Platform.FileSystem;
using XenForms.Core.Platform.Reflection;
using XenForms.Core.Platform.XAML;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Core.Toolbox.Workflow;
using XenForms.Core.XAML;
using XenForms.Toolbox.UI.Logging;
using XenForms.Toolbox.UI.Pouches.FilesFolder.Commands;
using XenForms.Toolbox.UI.Pouches.VisualTree;

namespace Toolbox.IoC
{
    public class CoreFxModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Logging>().To<DefaultToolboxLogging>();
            Bind<ToolboxLogging>().To<DefaultToolboxLogging>();
            Bind<IMessageBus>().To<DefaultMessageBus>().InSingletonScope();
            Bind<IXenMessageFinder>().To<XenMessageFinder>();
            Bind<PropertyEditorManager>().To<DefaultPropertyEditorManager>().InSingletonScope();
            Bind<ToolboxSocket>().To<DefaultToolboxSocket>().InSingletonScope();
            Bind<ToolboxSocketManager>().To<DefaultToolboxSocketManager>().InSingletonScope();
            Bind<INetworkAdapter>().To<NetworkAdapter>().InSingletonScope();
            Bind<ToolboxWorkflow>().To<DefaultToolboxWorkflow>().InSingletonScope();
            Bind<ToolboxAppEvents>().ToSelf().InSingletonScope();
            Bind<DesignerBridge>().ToSelf().InSingletonScope();
            Bind<ProjectWorkspace>().ToSelf().InSingletonScope();
            Bind<LoadProjectDialog>().ToSelf();
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IFileSystemTraverser>().To<FileSystemTraverser>();
            Bind<ITypeFinder>().To<TypeFinder>();
            Bind<IXamlPostProcessor>().To<XamlPostProcessor>().InSingletonScope();
            Bind<RegisterNewViewDialog>().ToSelf();
            Bind<ProjectState>().ToSelf().InSingletonScope();

            Bind<IFindCustomAttributes<PropertyEditorAttribute>>()
                .To<AppDomainCustomAttributeFinder<PropertyEditorAttribute>>()
                .InSingletonScope();

            Bind<IFindCustomAttributes<MenuPlacementAttribute>>()
                .To<AppDomainCustomAttributeFinder<MenuPlacementAttribute>>()
                .InSingletonScope();
        }
    }
}