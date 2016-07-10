using Ninject.Extensions.Conventions;
using Ninject.Modules;
using NLog;
using XenForms.Core;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Shell;

namespace XenForms.Windows
{
    public class WindowsModule : NinjectModule
    {
        private ILogger _logger;


        public override void Load()
        {
            Bind<ILogger>().ToMethod(p => LogManager.GetCurrentClassLogger());
            Bind<ISettingsStore>().To<WindowsSettingsStore>();
            Bind<XenFormsEnvironment>().To<WindowsXenFormsEnvironment>();

            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info("Scanning for startup tasks.");

            Kernel.Bind(x =>
            {
                _logger.Info("Scanning for startup tasks.");

                x.FromAssemblyContaining<MainForm>()
                    .SelectAllClasses()
                    .InheritedFrom<IStartupTask>()
                    .BindSingleInterface();

                _logger.Info("Startup task scan complete.");
            });

            _logger.Info("Startup task scan complete.");
        }
    }
}