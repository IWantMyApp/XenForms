namespace XenForms.Core.Messages
{
    /// <summary>
    /// A message from the toolbox requesting that a XAML document be loaded.
    /// </summary>
    public class OpenXamlRequest : Request
    {
        public string Xaml { get; set; }
        public string FileName { get; set; }
    }

    public class OpenXamlResponse : XamlResponse
    {
        public string FileName { get; set; }
    }
}