namespace XenForms.Core.Messages
{
    public enum EditCollectionType
    {
        Add = 0,
        Delete = 1
    }

    public class EditCollectionRequest : Request, IWidgetMessage
    {
        public EditCollectionType Type { get; set; }
        public string WidgetId { get; set; }
        public string[] Path { get; set; }
    }

    public class EditCollectionResponse : Response, IWidgetMessage
    {
        public bool Successful { get; set; }
        public EditCollectionType Type { get; set; }
        public string WidgetId { get; set; }
        public string Message { get; set; }
    }
}