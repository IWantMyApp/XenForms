using System.Linq;
using Xamarin.Forms;
using XenForms.Core.UI;

namespace XenForms.Designer.XamarinForms.UI
{
    public class BaseHierarchyTraverser : IXenWidgetHierarchy
    {
        public XenWidget Root { get; set; }

        public DesignerPair this[string id]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(id)) return null;

                VisualElement ve;

                if (_viewLookup.ContainsKey(id))
                {
                    ve = _viewLookup[id];
                }
                else
                {
                    return null;
                }

                // Placing the recursive lookup here to save cycles if the view is not "cached"
                var all = Root.GetNodeAndDescendants();
                var match = all.FirstOrDefault(w => w.Id != null && w.Id.Equals(id));

                var result = new DesignerPair
                {
                    XenWidget = match,
                    VisualElement = ve
                };

                return result;
            }
        }

        public XenWidget FromPage(ContentPage page)
        {
            var result = new XenWidget
            {
                Id = page.Id.ToString(),
                Type = page.GetType().Name,
                Name = page.Title
            };

            if (page.Content == null)
            {
                return result;
            }

            // Create the hierarchy from the views that are already attached to the page.
            Root = FromView(page.Content, result);
            return Root;
        }

        /// <summary>
        /// Remove the XenWidget from the hierarchy.
        /// </summary>
        /// <param name="widget"></param>
        /// <returns>
        /// Returns true if the <see cref="widget"/> has been removed; otherwise, false.
        /// </returns>
        public bool Remove(XenWidget widget)
        {
            return Remove(widget.Id);
        }

        /// <summary>
        /// Remove the XenWidget from the hierarchy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns true if the <see cref="XenWidget"/> has been removed; otherwise, false.
        /// </returns>
        public bool Remove(string id)
        {
            var pair = this[id];
            if (pair == null) return false;

            var child = pair.XenWidget;
            var parent = child.Parent;
            if (parent == null) return false;

            if (!parent.Children.Contains(child)) return false;

            parent.Children.Remove(child);

            return true;
        }

        /// <summary>
        /// Determine if the <param name="view"></param> has been added to hierarchy.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>
        /// Returns true if the view has been added; otherwise, false.
        /// </returns>
        internal bool IsAttached(View view)
        {
            if (view == null) return false;
            var id = view.Id.ToString();

            return _viewLookup.ContainsKey(id);
        }

        /// <summary>
        /// Return or create a XenWidget from the given view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parentWidget"></param>
        /// <returns></returns>
        private XenWidget Create(View view, XenWidget parentWidget)
        {
            if (view == null) return null;

            var id = view.Id.ToString();
            _viewLookup.Add(id, view);

            return new XenWidget
            {
                Id = id,
                Parent = parentWidget,
                Type = view.GetType().Name,
                Name = "Not implemented, yet."
            };
        }

        /// <summary>
        /// Create the XenWidget visual tree from a given view.
        /// </summary>
        /// <param name="childView">The child visual element</param>
        /// <param name="parentWidget">The visual elements parent XenWidget</param>
        /// <returns></returns>
        internal XenWidget FromView(View childView, XenWidget parentWidget)
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

            var childWidget = Create(childView, parentWidget);
            parentWidget.Children.Add(childWidget);

            /*
                3. Determine if the child view has children of it's own.
            */

            var isParentView = childView as ILayoutController;
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
                var grandchildView = grandchild as View;

                if (grandchildView != null)
                {
                    if (grandchildView is ILayoutController)
                    {
                        FromView(grandchildView, childWidget);
                    }
                    else
                    {
                        var grandchildWidget = Create(grandchildView, childWidget);
                        childWidget.Children.Add(grandchildWidget);
                    }
                }
            }

            return parentWidget;
        }
    }
}