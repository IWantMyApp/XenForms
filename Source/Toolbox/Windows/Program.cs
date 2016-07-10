using System;
using Eto;
using Eto.Forms;
using Ninject;
using Ninject.Modules;
using Toolbox.IoC;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell;

namespace XenForms.Windows
{
    public class Program
    {
        static Program()
        {
            var modules = new INinjectModule []
            {
                new CoreFxModule(),
                new ShellModule(),
                new WindowsModule(),
                new PluginModule()
            };

            ToolboxApp.Services = new StandardKernel(modules);
        }


        [STAThread]
        public static void Main(string[] args)
        {
            MainForm mainFrm = null;

            try
            {
                ToolboxApp.Log.Info("Application dependencies loaded.");

                var xf = new Application(Platform.Detect);
                mainFrm = ToolboxApp.Services.Get<MainForm>();
                xf.Run(mainFrm);
            }
            catch (Exception e)
            {
                ToolboxApp.Log.Error(e, "An unhandled exception occurred. The application will close.");
                var errorDialog = ErrorDialog.Create(e.ToString());

                if (mainFrm == null || !mainFrm.Loaded)
                {
                    errorDialog.ShowModal();
                }
                else
                {
                    errorDialog.ShowModal(mainFrm);
                }
            }
        }
    }
}