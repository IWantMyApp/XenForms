namespace XenForms.Core.Messages
{
    public class CreateStackLayoutRequest : Request
    {
        public string ParentId { get; set; }
        public string Orientation { get; set; }
        public double Spacing { get; set; }
    }
}