using System;
using Ninject;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell;

namespace XenForms.Toolbox.UI
{
    // ReSharper disable once InconsistentNaming
    public static partial class ToolboxUI
    {
        public static XenVisualElement Create(Type objType)
        {
            return ToolboxApp.Services.Get(objType) as XenVisualElement;
        }


        public static T Create<T>() where T : XenVisualElement
        {
            return ToolboxApp.Services.Get<T>();
        }


        public static T Activate<T>(T element) where T : XenVisualElement
        {
            if (element == null) return null;

            element.OnActivate();
            return element;
        }
    }
}