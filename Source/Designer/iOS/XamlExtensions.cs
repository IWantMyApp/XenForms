using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using XenForms.Core.XAML;

namespace XenForms.Designer.iOS
{
    public static class BindingObjectExtensions
    {
        private static readonly Func<BindableObject, string, Action<XamlParseErrorInfo>, BindableObject> LoadXaml;


        static BindingObjectExtensions()
        {
            // This is the current situation, where the LoadFromXaml is the only non-public static method.
            var genericMethod = typeof(Xamarin.Forms.Xaml.Extensions)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .FirstOrDefault();

            // If we didn't find it, it may be because the extension method may be public now :)
            if (genericMethod == null)
            {
                genericMethod = typeof(Xamarin.Forms.Xaml.Extensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.GetParameters().Last().ParameterType == typeof(string));
            }

            if (genericMethod == null)
            {
                LoadXaml = (view, xaml, onError) =>
                {
                    throw new NotSupportedException("Xamarin.Forms implementation of XAML loading not found.");
                };
            }
            else
            {
                genericMethod = genericMethod.MakeGenericMethod(typeof(BindableObject));
                LoadXaml = (view, xaml, onError) =>
                {
                    try
                    {
                        return (BindableObject) genericMethod.Invoke(null, new object[] {view, xaml});
                    }
                    catch (Exception e)
                    {
                        var info = new XamlParseErrorInfo
                        {
                            Message = e.InnerException?.Message ?? e.Message
                        };

                        onError?.Invoke(info);
                        return null;
                    }
                };
            }
        }


        /// <summary>
        /// Applies the given XAML to the view.
        /// </summary>
        public static TView Load<TView>(this TView view, string xaml, Action<XamlParseErrorInfo> onError = null) where TView : BindableObject
        {
            return (TView)LoadXaml(view, xaml, onError);
        }
    }
}
