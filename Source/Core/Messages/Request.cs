namespace XenForms.Core.Messages
{
    public abstract class Request : XenMessage
    {
        public virtual ValidationResult[] Validate(ValidationResult[] result)
        {
            return result;
        }
    }
}