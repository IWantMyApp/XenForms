using System;

namespace XenForms.Core.Toolbox.AppEvents
{
    public class ConstructorCalled : IAppEvent
    {
        public bool Successful { get; set; }

        public ConstructorCalled(bool successful)
        {
            Successful = successful;
        }
    }
}
