using System;
using System.Reflection;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class LoadProjectReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<LoadProjectRequest>();
            if (request == null) return;

            try
            {
                Assembly assembly;
                var data = Convert.FromBase64String(request.AssemblyData);
                Loader.Load(data, out assembly);
            }
            catch (Exception e)
            {
                DesignThread.Invoke(() =>
                {
                    App.ConnectionPage.ShowError(e);
                });     
            }
        }
    }
}