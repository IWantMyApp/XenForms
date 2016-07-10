using System;
using Xamarin.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    /// <summary>
    /// Implicitly assumes that the view has a default constructor w/o params.
    /// </summary>
    public class CreateWidgetReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var request = ctx.Get<CreateWidgetRequest>();
            if (request == null) return;

            var viewType = TypeFinder.Find(request.TypeName);

            if (viewType == null)
            {
                DesignThread.Invoke(() =>
                {
                    App.ConnectionPage.ShowError($"Unable to create {request.TypeName}. Have you loaded the project assemblies?");
                });

                return;
            }

            var view = Activator.CreateInstance(viewType) as View;
            if (view == null)
            {
                DesignThread.Invoke(() =>
                {
                    App.ConnectionPage.ShowError($"Error instantiating {request.TypeName}.");
                });

                return;
            }

            var target = Surface[request.ParentId];
            if (target == null) return;

            var attached = false;

            DesignThread.Invoke(() =>
            {
                attached = Surface.SetParent(view, target);
            });

            if (!attached) return;

            var pair = Surface[view.Id];

            var xamlDefaults = string.Empty;

            try
            {
                var newView = Activator.CreateInstance(viewType) as View;
                xamlDefaults = XamlWriter.Save(newView);
            }
            catch (Exception)
            {
                // ignored
            }

            ctx.SetResponse<CreateWidgetResponse>(r =>
            {
                r.XamlDefaults = new[] {xamlDefaults};
                r.Widget = pair.XenWidget;
                r.Parent = target.XenWidget;
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
