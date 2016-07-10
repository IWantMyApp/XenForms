namespace XenForms.Core.Toolbox.AppEvents
{
    public class ShowStatusMessage : IAppEvent
    {
        public const string Ready = "Ready";
        public string Message { get; set; }

        public ShowStatusMessage(string message)
        {
            Message = message;
        }
    }
}
