using System;
using Xamarin.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Reactions
{
    public class CreateGridReaction : XamarinFormsReaction
    {
        protected override void OnExecute(XenMessageContext ctx)
        {
            var req = ctx.Get<CreateGridRequest>();
            if (req == null) return;

            var target = Surface[req.ParentId];
            if (target == null) return;

            var attached = false;
            var rowCollection = new RowDefinitionCollection();
            var colCollection = new ColumnDefinitionCollection();

            for (var i = 0; i < req.Rows; i++)
            {
                var row = new RowDefinition();
                rowCollection.Add(row);
            }

            for (var j = 0; j < req.Columns; j++)
            {
                var col = new ColumnDefinition();
                colCollection.Add(col);
            }

            var view = new Grid
            {
                RowDefinitions = rowCollection,
                ColumnDefinitions = colCollection,
                ColumnSpacing = req.ColumnSpacing,
                RowSpacing = req.RowSpacing
            };

            DesignThread.Invoke(() =>
            {
                attached = Surface.SetParent(view, target);
            });

            if (!attached) return;

            var pair = Surface[view.Id];

            var xamlDefaults = string.Empty;

            try
            {
                var newGrid = new Grid();
                xamlDefaults = XamlWriter.Save(newGrid);
            }
            catch (Exception)
            {
                // ignored
            }

            ctx.SetResponse<CreateWidgetResponse>(res =>
            {
                res.XamlDefaults = new [] {xamlDefaults};
                res.Widget = pair.XenWidget;
                res.Parent = target.XenWidget;
                res.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
