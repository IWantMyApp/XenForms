namespace XenForms.Core.Messages
{
    public class DeleteWidgetRequest : Request, IWidgetMessage
    {
        public string WidgetId { get; set; }
    }
}
