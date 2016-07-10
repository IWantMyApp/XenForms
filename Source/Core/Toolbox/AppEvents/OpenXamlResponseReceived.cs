namespace XenForms.Core.Toolbox.AppEvents
{
    public class OpenXamlResponseReceived : IAppEvent
    {
        public string[] Xaml { get; set; }
        public string FileName { get; set; }

        public OpenXamlResponseReceived(string[] xaml, string fileName)
        {
            Xaml = xaml;
            FileName = fileName;
        }
    }
}