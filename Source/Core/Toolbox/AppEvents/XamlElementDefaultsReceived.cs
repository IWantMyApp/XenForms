namespace XenForms.Core.Toolbox.AppEvents
{
    public class XamlElementDefaultsReceived : IAppEvent
    {
        public string[] Xaml { get; set; }
        public XamlElementDefaultsReceived(string[] xaml)
        {
            Xaml = xaml;
        }
    }
}
