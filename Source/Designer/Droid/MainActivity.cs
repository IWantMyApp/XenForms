using Android.App;
using Android.Content.PM;
using Android.OS;
using Ninject;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XenForms.Designer.XamarinForms.UI;


namespace XenForms.Designer.XamarinForms.Droid
{
    [Activity(Name = "com.xenforms.designer.MainActivity" , Label = "XenForms", Icon = "@drawable/icon", Theme = "@style/Theme.XenForms", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        static MainActivity()
        {
            App.Kernel = new StandardKernel(new DroidModule());
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetIcon(Android.Resource.Color.Transparent);

            Forms.Init(this, bundle);
            var app = App.Kernel.Get<App>();
            LoadApplication(app);
        }
    }
}