using System.Linq;
using Xamarin.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.Tests.EmptyFakes
{
    sealed class FakeAttachReaction : XamarinFormsReaction
    {
        public Entry Child { get; set; }


        /// <summary>
        /// Returns true if the attach method succeeded; otherwise, false.
        /// </summary>
        public bool AttachResult { get; set; }


        /// <summary>
        /// Returns true if the hierarchy contains the child's XenWidget
        /// </summary>
        public bool HierarchyContainsChild { get; set; }


        /// <summary>
        /// Return true if the child has been attached to the correct XenWidget
        /// </summary>
        public bool ChildHasCorrectParent { get; set; }


        protected override void OnExecute(XenMessageContext ctx)
        {
            /*
                Document Outline:

                ContentPage
                    StackLayout
                        Entry *(this is done here)*
            */

            AttachResult = Surface.SetParent(Child, Surface.Root.Children[0]);
            var widgets = Surface.Root.GetNodeAndDescendants();
            HierarchyContainsChild = widgets.Any(w => w.Id == Child.Id.ToString());

            if (Surface.Root.Children[0].Children[0].Type == nameof(Entry))
            {
                ChildHasCorrectParent = true;
            }
        }
    }
}