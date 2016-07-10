using System;
using System.Collections.Generic;
using System.Linq;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// A UI command to be executed on the design surface.
    /// To be invoked, subclasses must register via <see cref="Reaction.Register{TRequestType,TDesignerAction}(System.Func{TDesignerAction})"/>
    /// with an associated <see cref="XenMessage"/> type. When the specific message is received, the registered action will be executed.
    /// </summary>
    public abstract class Reaction
    {
        #region Registration

        internal static Dictionary<Type, Type> Actions { get; }
        internal static Dictionary<Type, Func<Reaction>> Creators { get; } 


        static Reaction()
        {
            Actions = new Dictionary<Type, Type>();
            Creators = new Dictionary<Type, Func<Reaction>>();
        }


        public static void Reset()
        {
            Actions.Clear();
            Creators.Clear();
            GetServices = null;
        }


        /// <summary>
        /// Inject dependencies into newly created object.
        /// </summary>
        public static Action<object> GetServices;


        /// <summary>
        /// Overload allows a specified constructor to be invoked each time the designer action is received.
        /// When the specified <see cref="XenMessage"/> is receieved from the toolbox, invoke the
        /// specified <see cref="TReaction"/>.
        /// The action is in memory as long as the request is executing.
        /// </summary>
        /// <typeparam name="TRequestType">Usually a toolbox request</typeparam>
        /// <typeparam name="TReaction">Designer Action</typeparam>
        /// <param name="creation">Factory method</param>
        /// <returns>True if the action was executed; otherwise, false.</returns>
        public static bool Register<TRequestType, TReaction>(Func<TReaction> creation) where TRequestType : XenMessage where TReaction : Reaction
        {
            if (Creators.ContainsKey(typeof (TReaction)))
            {
                return false;
            }

            Register<TRequestType, TReaction>();

            lock (Creators)
            {
                Creators.Add(typeof (TReaction), creation);
            }

            return true;
        }


        /// <summary>
        /// When the specified <see cref="XenMessage"/> is receieved from the toolbox, invoke the
        /// specified <see cref="TDesignerReaction"/>.
        /// The action is in memory as long as the request is executing.
        /// </summary>
        /// <typeparam name="TRequestType">Usually a toolbox request</typeparam>
        /// <typeparam name="TDesignerReaction">Designer Action</typeparam>
        /// <returns>True if the action was executed; otherwise, false.</returns>
        public static bool Register<TRequestType, TDesignerReaction>() where TRequestType : XenMessage where TDesignerReaction : Reaction
        {
            if (Actions.ContainsKey(typeof (TRequestType)))
            {
                return false;
            }

            lock (Actions)
            {
                Actions.Add(typeof (TRequestType), typeof (TDesignerReaction));
            }

            return true;
        }


        private static Reaction Create(Type requestType)
        {
            if (!Actions.ContainsKey(requestType))
            {
                return null;
            }

            Reaction result;
            Func<Reaction> creator = null;

            var designerAction = Actions[requestType];
            if (designerAction == null) return null;

            if (Creators.ContainsKey(designerAction))
            {
                creator = Creators[designerAction];
            }

            try
            {
                if (creator == null)
                {
                    result = Activator.CreateInstance(designerAction) as Reaction;
                }
                else
                {
                    result = creator();
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"No parameterless constructor was found. Could not create {designerAction.Name}.");
            }
         
            return result;
        }


        #endregion

        /// <summary>
        /// Execute a <see cref="Reaction"/> with the given <paramref name="context"/>.
        /// </summary>
        /// <param name="reaction">A <see cref="Reaction"/> object.</param>
        /// <param name="context">An object containing a request and response.</param>
        /// <returns>
        /// Returns true when a <see cref="Reaction"/> has been executed; otherwise, false.
        /// </returns>
        public static bool Execute(Reaction reaction, XenMessageContext context)
        {
            if (reaction == null) throw new ArgumentNullException(nameof(reaction));

            // Inject dependencies
            GetServices?.Invoke(reaction);
            reaction.OnExecute(context);
            return true;
        }


        /// <summary>
        /// Execute a <see cref="Reaction"/> with the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An object containing a request and response.</param>
        /// <returns>
        /// Returns true when a <see cref="Reaction"/> has been executed; otherwise, false.
        /// </returns>
        public static bool Execute(XenMessageContext context)
        {
            var actionName = context?.Request?.Action;
            if (actionName == null) return false;

            // todo: log this condition
            if (Actions == null || Actions.Count == 0) return false;
            var requestType = Actions.Keys.FirstOrDefault(k => k.Name != null && k.Name.Equals(actionName));

            if (requestType == null) return false;

            var actionObj = Create(requestType);
            if (actionObj == null) return false;

            Execute(actionObj, context);
            return true;
        }


        /// <summary>
        /// Perform any custom code that should be executed when a request is received.
        /// This method will only be invoked, if it has been registered through a <see cref="Register{TRequestType,TDesignerAction}()"/> overload.
        /// </summary>
        /// <param name="ctx">An object containing a request and response.</param>
        protected abstract void OnExecute(XenMessageContext ctx);
    }
}
