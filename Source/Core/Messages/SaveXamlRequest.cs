namespace XenForms.Core.Messages
{
    public class SaveXamlRequest : Request {}

    public class SaveXamlResponse : Response
    {
        public bool Successful { get; set; }
        public string Xaml { get; set; }
        public string Error { get; set; }
    }
}
