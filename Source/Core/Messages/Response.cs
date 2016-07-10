namespace XenForms.Core.Messages
{
    public abstract class Response : XenMessage
    {
        public bool UnhandledExceptionOccurred { get; set; }
        public string ExceptionMessage { get; set; }
    }
}