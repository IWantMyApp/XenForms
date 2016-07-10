using Android.App;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.XamarinForms.Droid
{
    public class GetDesignerVersion : IGetDesignerVersion
    {
        public string GetVersion()
        {
            var context = Application.Context.ApplicationContext;
            var code = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
            return code.ToString();
        }
    }
}