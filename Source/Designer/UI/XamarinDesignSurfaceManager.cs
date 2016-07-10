using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI;

[assembly:Dependency(typeof(XamarinDesignSurfaceManager))]
namespace XenForms.Designer.XamarinForms.UI
{
    public class XamarinDesignSurfaceManager : DesignSurfaceManager<VisualElement>
    {
        private readonly Dictionary<string, VisualElement> _lookup;


        public XamarinDesignSurfaceManager()
        {
            _lookup = new Dictionary<string, VisualElement>();
        }


        public override DesignSurfacePair<VisualElement> this[string id]
        {
            get
            {
                // No hierarchy exists, so we return null.
                if (Root == null)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(id)) return null;

                VisualElement ve;

                if (_lookup.ContainsKey(id))
                {
                    ve = _lookup[id];
                }
                else
                {
                    return null;
                }

                // Placing the recursive lookup here to save cycles if the view is not "cached"
                var all = Root.GetNodeAndDescendants();
                var match = all.FirstOrDefault(w => w.Id != null && w.Id.Equals(id));

                var result = new DesignSurfacePair<VisualElement>
                {
                    XenWidget = match,
                    VisualElement = ve
                };

                return result;
            }
        }


        public override DesignSurfacePair<VisualElement> this[Guid id] => this[id.ToString()];


        public override bool Remove(XenWidget widget)
        {
            return Remove(widget.Id);
        }


        public override bool Remove(string id)
        {
            var pair = this[id];
            if (pair == null) return false;

            if (pair.XenWidget == Root)
            {
                throw new InvalidOperationException($"You cannot remove the root widget through this method. Call {nameof(SetDesignSurface)} instead.");
            }

            var xenRemoved = RemoveXenWidget(pair);
            var xamRemoved = RemoveVisualElement(pair);

            return xenRemoved && xamRemoved;
        }


        public override XenWidget SetDesignSurface(VisualElement view)
        {
            var page = view as ContentPage;

            if (page == null)
            {
                Root = null;
                return null;
            }

            var widget = CreateXenWidget(view, null);
            widget.Name = page.Title;
            Root = widget;

            // you can't remove the root widget
            Root.CanDelete = false;

            // the root widget must have children
            Root.IsLayout = true;

            // short-circuit if there's no children to find.
            if (page.Content == null)
            {
                return widget;
            }

            // Create the hierarchy from the views that are already attached to the page.
            Root = BuildTree(page.Content, widget);

            return Root;
        }


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public override bool SetParent(VisualElement child, VisualElement parent)
        {
            if (child == null || parent == null) return false;

            if (!Contains(parent))
            {
                throw new InvalidOperationException("The destination must have a corresponding XenWidget attached to the hierarchy.");
            }

            var parentType = parent.GetType();
            var parentPair = this[parent.Id];
            var childView = child as View;

            var page = parentPair.VisualElement as ContentPage;

            // Check if the parent is the Root ContentPage.
            if (page != null && page == parentPair.VisualElement)
            {
                if (childView == null) return false;
                page.Content = childView;
                SetDesignSurface(page);
                return true;
            }

            // Check if it's possible to add and remove child views from this container type.
            var parentContainer = parent as IViewContainer<View>;
            if (parentContainer != null)
            {
                var result = BuildTree(child, parentPair.XenWidget);
                if (result == null) return false;

                parentContainer.Children.Add(childView);
                return true;
            }

            // check if the parent has a content property, like a ScrollView or ContentView
            var contentAttribute = parentType.GetCustomAttributes(typeof (ContentPropertyAttribute), true);
            if (contentAttribute.Length > 0)
            {
                var attribute = contentAttribute[0] as ContentPropertyAttribute;

                // Check that the Content property name is set.
                if (!string.IsNullOrWhiteSpace(attribute?.Name))
                {
                    var property = parentType.GetProperty(attribute.Name);
                    if (property.PropertyType == typeof(View))
                    {
                        var result = BuildTree(child, parentPair.XenWidget);

                        if (result == null) return false;
                        property.SetValue(parent, childView);
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public override bool SetParent(VisualElement child, XenWidget parent)
        {
            if (child == null || parent == null) return false;

            // Returns null when id not found.
            var pair = this[parent.Id];
            if (pair == null) return false;

            return SetParent(child, pair.VisualElement);
        }


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public override bool SetParent(VisualElement child, DesignSurfacePair<VisualElement> parent)
        {
            return SetParent(child, parent.VisualElement);
        }


        public override bool Contains(XenWidget widget)
        {
            if (string.IsNullOrWhiteSpace(widget?.Id)) return false;
            return _lookup.ContainsKey(widget.Id);
        }


        public override bool Contains(VisualElement view)
        {
            if (view == null) return false;
            var id = view.Id.ToString();

            return _lookup.ContainsKey(id);
        }


        protected override XenWidget CreateXenWidget(VisualElement source, XenWidget parent)
        {
            if (source == null) return null;

            var id = source.Id.ToString();
            if (!_lookup.ContainsKey(id))
            {
                _lookup.Add(id, source);
            }

            return new XenWidget
            {
                Id = id,
                Parent = parent,
                Type = source.GetType().Name,
                FullTypeName = source.GetType().FullName,
                Name = "Not implemented, yet.",
                CanDelete = parent != null
            };
        }


        protected override XenWidget BuildTree(VisualElement childView, XenWidget parentWidget)
        {
            /* 
                1. Return parent widget if child view is null.
                Since, We won't be able to create a XenWidget out of the view.
            */

            if (childView == null)
            {
                return parentWidget;
            }

            /*
                2. Create a XenWidget for the child view.
            */

            var childWidget = CreateXenWidget(childView, parentWidget);
            parentWidget.Children.Add(childWidget);

            if (ElementHelper.HasContentProperty(childView))
            {
                childWidget.AllowsManyChildren = ElementHelper.ContentPropertyAllowsManyChildren(childView);
                childWidget.IsContentPropertyViewType = ElementHelper.IsContentPropertyView(childView);
                childWidget.ContentPropertyTypeName = ElementHelper.GetContentPropertyTypeName(childView);
                childWidget.HasContentProperty = true;
                childWidget.IsLayout = false;
            }
            else
            {
                childWidget.HasContentProperty = false;
            }

            /*
                3. Determine if the child view has children of it's own.
            */

            var isParentView = childView as ILayoutController;
            if (isParentView != null)
            {
                childWidget.IsLayout = true;
            }

            var grandchildren = isParentView?.Children;

            if (grandchildren == null || grandchildren.Count == 0)
            {
                return parentWidget;
            }

            /*
                4. The child does have it's own children.
            */

            foreach (var grandchild in grandchildren)
            {
                var view = grandchild as View;

                if (view != null)
                {
                    if (view is ILayoutController)
                    {
                        BuildTree(view, childWidget);
                    }
                    else
                    {
                        var grandchildWidget = CreateXenWidget(view, childWidget);

                        if (ElementHelper.HasContentProperty(view))
                        {
                            grandchildWidget.IsLayout = true;
                        }

                        childWidget.Children.Add(grandchildWidget);
                    }
                }
            }

            return parentWidget;
        }


        private bool RemoveXenWidget(DesignSurfacePair<VisualElement> pair)
        {
            var child = pair.XenWidget;
            var parent = child.Parent;

            if (parent == null) return false;
            if (!parent.Children.Contains(child)) return false;

            parent.Children.Remove(child);
            return true;
        }


        private bool RemoveVisualElement(DesignSurfacePair<VisualElement> pair)
        {
            // we don't allow the root element to be removed.
            if (pair.XenWidget.Parent == null) return false;

            var parentId = pair.XenWidget.Parent.Id;
            var parentPair = this[parentId];

            if (parentPair == null) return false;

            // this is the only child of the content page
            if (parentId == Root.Id)
            {
                var page = parentPair.VisualElement as ContentPage;
                if (page != null)
                {
                    page.Content = null;
                    return true;
                }
            }

            // this maybe a layout control
            var viewContainer = parentPair.VisualElement as IViewContainer<View>;
            var childView = pair.VisualElement as View;
            var parentView = parentPair.VisualElement;

            // we can only remove view types; this shouldn't happen.
            if (childView == null) return false;

            // is this a content property?
            if (viewContainer == null && ElementHelper.HasContentProperty(parentView))
            {
                ElementHelper.ClearContentProperty(parentView);
            }

            // is it a layout control? i.e. stacklayout, grid..
            if (viewContainer == null) return false;

            // made it this far, it's a layout control.
            viewContainer.Children.Remove(childView);
            return true;
        }
    }
}