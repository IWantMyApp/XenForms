namespace XenForms.Toolbox.UI.Shell
{
    public static class XenVisualElementExtensions
    {
        public static T Activate<T>(this T element) where T : XenVisualElement
        {
            return ToolboxUI.Activate(element);
        }
    }
}
