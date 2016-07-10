using System;
using Xamarin.Forms;
using XenForms.Core.XAML;
using XenForms.Designer.XamarinForms.UI;

namespace XenForms.Designer.XamarinForms.Droid
{
    public class XamlLoader : IXamlLoader
    {
        public TView Load<TView>(TView view, string xaml, Action<XamlParseErrorInfo> onError = null) where TView : BindableObject
        {
            return view.Load(xaml, onError);
        }
    }
}