namespace XenForms.Core.Toolbox
{
    public interface IToolboxEventModel
    {
        string EventHandlerName { get; set; }
        string SourceFile { get; set; }
        string DisplayName { get; set; }
        ToolboxEventVisibility Visibility { get; set; }
    }
}