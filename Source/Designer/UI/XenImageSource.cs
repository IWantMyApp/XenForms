using Xamarin.Forms;

namespace XenForms.Designer.XamarinForms.UI
{
    public sealed class XenImageSource : StreamImageSource
    {
        public string FileName { get; set; }


        public XenImageSource(ImageSource source)
        {
            var sis = source as StreamImageSource;

            if (sis != null)
            {
                Stream = sis.Stream;
            }
        }
    }
}