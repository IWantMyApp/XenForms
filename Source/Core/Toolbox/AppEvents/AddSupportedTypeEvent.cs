namespace XenForms.Core.Toolbox.AppEvents
{
    public class AddSupportedTypeEvent : IAppEvent
    {
        public bool Added { get; set; }
        public bool AlreadyRegistered { get; set; }
        public string DisplayMessage { get; set; }
    }
}