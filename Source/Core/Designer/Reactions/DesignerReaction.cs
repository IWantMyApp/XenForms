using System;
using Ninject;
using XenForms.Core.Messages;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;
using XenForms.Core.XAML;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// An operation performed on a design surface.
    /// </summary>
    /// <typeparam name="TVisualElement">A type common to all UI elements on the page.</typeparam>
    public abstract class DesignerReaction<TVisualElement> : Reaction
    {
        protected DesignerReaction() { }
        protected DesignerReaction(TVisualElement rootView)
        {
            RootView = rootView;
        }


        protected DesignSurfaceManager<TVisualElement> Surface { get; private set; }
        protected IDesignThread DesignThread { get; private set; }
        protected XenWidget Root { get; private set; }
        protected TVisualElement RootView { get; private set; }
        protected IXenCodeLoader Loader { get; private set; }
        protected TypeRegistrar SupportingTypes => TypeRegistrar.Instance;
        protected ITypeFinder TypeFinder { get; private set; }
        protected XenXamlWriter XamlWriter { get; private set; }

        private DescriptorRegistrar _dr;
        protected DescriptorRegistrar Descriptor
        {
            get
            {
                if (_dr == null)
                {
                    throw new InvalidOperationException($"{nameof(Initialize)} must be called, first.");
                }

                return _dr;
            }
        }


        [Inject]
        public void Initialize(ITypeFinder typeFinder, IDesignThread thread, IXenCodeLoader loader, DesignSurfaceManager<TVisualElement> surface)
        {
            if (RootView == null)
            {
                throw new InvalidOperationException($"The class was registered with the wrong {nameof(Register)} overload. No {RootView} was assigned.");
            }

            _dr = DescriptorRegistrar.Create(typeFinder);
            XamlWriter = new XenXamlWriter();

            TypeFinder = typeFinder;
            Surface = surface;
            DesignThread = thread;
            DesignThread.Context = RootView;
            Loader = loader;
            Root = Surface.SetDesignSurface(RootView);
        }


        /// <summary>
        /// Create an association between a request and an action to be performed on the designer.
        /// The <paramref name="page"/> parameter will be the root view for the designer action.
        /// </summary>
        /// <typeparam name="TRequestType"></typeparam>
        /// <typeparam name="TDesignerReaction"></typeparam>
        /// <param name="page"></param>
        public static void Register<TRequestType, TDesignerReaction>(TVisualElement page) where TDesignerReaction : DesignerReaction<TVisualElement>, new() where TRequestType : XenMessage
        {
            Register<TRequestType, TDesignerReaction>(() => new TDesignerReaction { RootView = page });
        }
    }
}