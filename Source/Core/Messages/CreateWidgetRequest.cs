namespace XenForms.Core.Messages
{
    public class CreateWidgetRequest : Request
    {
        public string ParentId { get; set; }
        public string TypeName { get; set; }
    }

    public class CreateWidgetResponse : XamlResponse {}
}