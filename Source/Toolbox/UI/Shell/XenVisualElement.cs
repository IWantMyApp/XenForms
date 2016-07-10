using System.Threading.Tasks;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Shell
{
    public abstract class XenVisualElement
    {
        public Control View { get; private set; }

        
        internal void OnActivate()
        {
            View = OnDefineLayout();
            View.PreLoad += (s, e) => OnBeforeLayoutShown();
            View.PreLoad += async (s, e) => await OnBeforeLayoutShownAsync();
            View.SizeChanged += (s, e) => OnLayoutShown();
            View.UnLoad += (s, e) => OnUnload();

            ToolboxApp.Log.Trace("{0} has been activated.", GetType().Name);
        }


        protected virtual void OnLayoutShown() { }
        protected virtual void OnBeforeLayoutShown() { }
        protected virtual void OnUnload() { }

        protected async Task OnBeforeLayoutShownAsync()
        {
            await Task.FromResult<object>(null);
        }


        protected abstract Control OnDefineLayout();
    }
}