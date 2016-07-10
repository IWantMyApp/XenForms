namespace XenForms.Core.Toolbox.AppEvents
{
    public class SaveXamlReceived : IAppEvent
    {
        public string Error { get; set; }
        public bool Successful { get; set; }
        public string Xaml { get; set; }
    }
}