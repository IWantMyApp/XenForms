using XenForms.Core.Messages;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class EditCollectionResponseReceived : IAppEvent
    {
        public EditCollectionResponseReceived(EditCollectionType type, bool successful, string message)
        {
            Type = type;
            Successful = successful;
            Message = message;
        }

        public bool Successful { get; set; }
        public string Message { get; set; }
        public EditCollectionType Type { get; set; }
    }
}
