using Ninject.Modules;
using XenForms.Core.Platform.Reflection;
using XenForms.Core.Reflection;
using XenForms.Toolbox.UI.Pouches.VisualTree;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.MenuItems;

namespace Toolbox.IoC
{
    public class ShellModule : NinjectModule
    {
        public override void Load()
        {
            Bind<VisualTreePouch>().ToSelf().InTransientScope();
            Bind<DisconnectCommand>().ToSelf().InSingletonScope();
            Bind<ConnectCommand>().ToSelf().InSingletonScope();

            Bind<SettingsDialog>().ToSelf().InTransientScope();
            Bind<SettingsCommand>().ToSelf().InTransientScope();
            Bind<IFindCustomAttributes<SettingsPanelAttribute>>()
                .To<AppDomainCustomAttributeFinder<SettingsPanelAttribute>>()
                .InSingletonScope();
        }
    }
}