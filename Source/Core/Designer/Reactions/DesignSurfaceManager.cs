using System;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// <see cref="Reaction"/>'s can walk the visual tree, or document outline, of the design surface.
    /// When targeting a UI framework, this class must be extended with the details that are
    /// specific to the targeted UI framework.
    /// </summary>
    /// <typeparam name="TVisualElement">A type common to all child widgets on the design surface.</typeparam>
    public abstract class DesignSurfaceManager<TVisualElement>
    {
        /// <summary>
        /// A reference to the top-most UI element, usually a page.
        /// </summary>
        public XenWidget Root { get; set; }


        /// <summary>
        /// Checks whether or not the <paramref name="view"/> is a descendant of <see cref="Root"/>.
        /// </summary>
        /// <param name="view"></param>.
        /// <returns></returns>
        public abstract bool Contains(TVisualElement view);


        /// <summary>
        /// Checks whether or not the <paramref name="widget"/> is a descendant of <see cref="Root"/>.
        /// </summary>
        /// <param name="widget"></param>.
        /// <returns></returns>
        public abstract bool Contains(XenWidget widget);


        /// <summary>
        /// Return a <see cref="DesignSurfacePair{TVisualElement}"/> for the requested <paramref name="id"/>
        /// </summary>
        /// <param name="id">The <see cref="XenWidget.Id"/></param>
        /// <returns>
        /// The return type includes the <see cref="XenWidget"/> and target framework's base widget type, i.e. VisualElement, View, etc.
        /// </returns>
        public abstract DesignSurfacePair<TVisualElement> this[string id] { get; }


        /// <summary>
        /// Return a <see cref="DesignSurfacePair{TVisualElement}"/> for the requested <paramref name="id"/>
        /// </summary>
        /// <param name="id">The <see cref="XenWidget.Id"/></param>
        /// <returns>
        /// The return type includes the <see cref="XenWidget"/> and target framework's base widget type, i.e. VisualElement, View, etc.
        /// </returns>
        public abstract DesignSurfacePair<TVisualElement> this[Guid id] { get; }


        /// <summary>
        /// Remove the <paramref name="widget"/> from the visual tree.
        /// </summary>
        /// <param name="widget"></param>
        /// <returns>
        /// Returns true if the <paramref name="widget"/> has been removed; otherwise, false.
        /// </returns>
        public abstract bool Remove(XenWidget widget);


        /// <summary>
        /// Remove the <see cref="XenWidget"/> from the visual tree.
        /// </summary>
        /// <returns>
        /// Returns true if the <see cref="id"/> is a tracked <see cref="XenWidget"/> and it has been removed
        /// from the visual tree; otherwise, false.
        /// </returns>
        public abstract bool Remove(string id);


        /// <summary>
        /// Using the <paramref name="view"/> as the root element of a visual tree, recursively record and traverse
        /// each child element.
        /// </summary>
        /// <param name="view">The top-most parent widget. Usually a "page" type that can be pushed on a navigation stack.</param>
        /// <returns>
        /// Returns the top-most element of a visual tree.
        /// </returns>
        public abstract XenWidget SetDesignSurface(TVisualElement view);


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public abstract bool SetParent(TVisualElement child, TVisualElement parent);


        /// <summary>
        /// Attach the child to the parent object.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if the view was attached to the parent; otherwise, false.
        /// </returns>
        public abstract bool SetParent(TVisualElement child, XenWidget parent);


        /// <summary>
        /// Attach the child to the parent object.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if the view was attached to the parent; otherwise, false.
        /// </returns>
        public abstract bool SetParent(TVisualElement child, DesignSurfacePair<TVisualElement> parent);


        /// <summary>
        /// The target framework implementations should define this method.
        /// Using the <paramref name="source"/> the method should instantiate and return a <see cref="XenWidget"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected abstract XenWidget CreateXenWidget(TVisualElement source, XenWidget parent);


        /// <summary>
        /// Using the <paramref name="childView"/> as a starting element, recursively record and traverse each child element.
        /// </summary>
        /// <param name="childView">The target framework's UI element.</param>
        /// <param name="parentWidget">The <paramref name="childView"/>'s parent.</param>
        /// <returns>
        /// Returns the a XenForms representation of the <paramref name="childView" />.
        /// </returns>
        protected abstract XenWidget BuildTree(TVisualElement childView, XenWidget parentWidget);
    }
}