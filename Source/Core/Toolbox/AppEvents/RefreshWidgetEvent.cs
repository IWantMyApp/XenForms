namespace XenForms.Core.Toolbox.AppEvents
{
    public class RefreshWidgetEvent : IAppEvent
    {
        public int Row { get; set; }
        public RefreshWidgetEvent(int row)
        {
            Row = row;
        }
    }
}
