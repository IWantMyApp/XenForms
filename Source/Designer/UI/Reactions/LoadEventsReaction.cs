using System;
using System.Reflection;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class LoadEventsReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<LoadEventsRequest>();
            if (request == null) return;

            /*
            Assembly assembly;
            var data = Convert.FromBase64String(request.AssemblyData);
            Loader.Load(data, out assembly);
            */
        }
    }
}