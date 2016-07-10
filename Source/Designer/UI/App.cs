using System.Diagnostics;
using Ninject;
using Xamarin.Forms;
using XenForms.Core.Designer.Workflow;
using XenForms.Core.Networking;
using XenForms.Designer.XamarinForms.UI.Pages;

namespace XenForms.Designer.XamarinForms.UI
{
    public class App : Application
    {
        public const string XenClassId = "designer.xenforms.com";
        public static IKernel Kernel { get; set; }
        public static ConnectionPage ConnectionPage { get; set; }
        public static Page CurrentDesignSurface { get; set; }
        private DesignServer _server;
        private DesignWorkflow _workflow;


        public App()
        {
            GetDependencies();
            InitializeDesigner();

            ConnectionPage = Kernel.Get<ConnectionPage>();
            MainPage = new NavigationPage(ConnectionPage)
            {
                ClassId = XenClassId
            };
        }


        private void GetDependencies()
        {
            _server = Kernel.Get<DesignServer>();
            _workflow = Kernel.Get<DesignWorkflow>();
        }


        private void InitializeDesigner()
        {
            _server.Trace += (sender, args) =>
            {
                Debug.WriteLine($"{args.Level}: {args.Description}");
            };

            _server.Message += (sender, args) =>
            {
                _workflow.Queue(args.Message);
            };

            _server.ServerStarted += (sender, args) =>
            {
                _workflow.Start();
            };

            _server.ToolboxDisconnected += (senders, args) =>
            {
                _workflow.Shutdown();
            };

            DesignerAppEvents.SetupTypeDescriptors();
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }


        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
