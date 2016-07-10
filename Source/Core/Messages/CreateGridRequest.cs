namespace XenForms.Core.Messages
{
    public class CreateGridRequest : Request
    {
        public string ParentId { get; set; }
        public int Columns { get; set; }
        public int ColumnSpacing { get; set; }
        public int Rows { get; set; }
        public int RowSpacing { get; set; }
    }
}