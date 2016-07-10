using System;
using Xamarin.Forms;
using XenForms.Core.XAML;

namespace XenForms.Designer.XamarinForms.UI
{
    public interface IXamlLoader
    {
        TView Load<TView>(TView view, string xaml, Action<XamlParseErrorInfo> onError = null) where TView : BindableObject;
    }
}