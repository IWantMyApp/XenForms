using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;

namespace XenForms.Core.Designer.Workflow
{
    /// <summary>
    /// Responsible for dispatching toolbox messages to the appropriate <see cref="Reaction"/>.
    /// </summary>
    public abstract class DesignWorkflow
    {
        protected DesignServer Server { get; set; }


        protected DesignWorkflow(DesignServer server)
        {
            Server = server;
        }


        /// <summary>
        /// Queue a toolbox request to be executed by the designer. It does not have to be a request to get or set visual element data.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Queue(string message);


        /// <summary>
        /// Start the workflow.
        /// </summary>
        public abstract void Start();


        /// <summary>
        /// Shutdown the workflow.
        /// </summary>
        public abstract void Shutdown();
    }
}
