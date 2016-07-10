using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using NLog;
using XenForms.Core.Plugin;
using XenForms.Core.Reflection;
using XenForms.Toolbox.UI.Plugins.XamarinForms;

namespace XenForms.Windows
{
    public class PluginModule : NinjectModule
    {
        private ILogger _logger;


        public override void Load()
        {
            _logger = LogManager.GetCurrentClassLogger();

            IEnumerable<IBinding> bindings = null;
            var pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (!Directory.Exists(pluginDirectory))
            {
                _logger.Info($"The plugins folder '{pluginDirectory}' does not exist. Creating.");
                Directory.CreateDirectory(pluginDirectory);
            }

            Kernel.Bind(x =>
            {
                _logger.Info("Scanning for plugins.");

                try
                {
                    x.FromAssembliesInPath(pluginDirectory, ReflectionMethods.IsXenFormsDiscoverable)
                        .SelectAllClasses()
                        .InheritedFrom<IPluginRegistration>()
                        .BindSingleInterface();

                    x.FromAssemblyContaining<PluginRegistration>()
                        .SelectAllClasses()
                        .InheritedFrom<IPluginRegistration>()
                        .BindSingleInterface();

                    bindings = Kernel.GetBindings(typeof(IPluginRegistration));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    if (bindings != null)
                    {
                        var found = bindings.Any()
                            ? "Plugin registrations found."
                            : "No plugin registrations found.";

                        _logger.Info(found);
                    }

                    _logger.Info("Scanning for plugins complete.");
                }
            });
        }
    }
}