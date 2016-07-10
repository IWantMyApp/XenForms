using Foundation;
using Ninject;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        static AppDelegate()
        {
            App.Kernel = new StandardKernel(new iOSModule());
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            var xenforms = App.Kernel.Get<App>();
            LoadApplication(xenforms);

            return base.FinishedLaunching(app, options);
        }
    }
}
