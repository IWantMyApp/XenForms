using System.Threading.Tasks;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Shell
{
    public abstract class XenForm : XenVisualElement
    {
        private bool _saving;


        public async Task<bool> ShowFormAsync(object data = null)
        {
            await OnShowFormAsync(data);
            ToolboxApp.Log.Trace($"Showing form {GetType().Name}");
            return _saving;
        }


        public async Task CloseFormAsync(bool save = false)
        {
            _saving = save;

            await OnCloseFormAsync(save);
            ToolboxApp.Log.Trace($"Closing form {GetType().Name}");
        }


        protected virtual async Task OnShowFormAsync(object data = null)
        {
            await Task.FromResult(data);
        }


        protected virtual async Task OnCloseFormAsync(bool save = false)
        {
            await Task.FromResult(save);
        }
    }
}