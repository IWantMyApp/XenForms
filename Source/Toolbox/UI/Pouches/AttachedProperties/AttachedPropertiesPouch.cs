using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Pouches.Properties;
using XenForms.Toolbox.UI.Shell.Pouches;

namespace XenForms.Toolbox.UI.Pouches.AttachedProperties
{
    public class AttachedPropertiesPouch : ToolbeltPouch
    {
        private PropertyEditorGridView _grid;
        private Label _pouchHeader;


        public AttachedPropertiesPouch() : base("Attached Properties")
        {
            // Wait for the toolbox to receive and parse the widget's properties before attempting to show them.
            ToolboxApp.Bus.Listen<ShowAttachedProperties>(e =>
            {
                Application.Instance.AsyncInvoke(() =>
                {
                    var result = _grid.ShowEditors(ToolboxApp.Project.State.SelectedVisualTreeNode,
                        e.Widget.AttachedProperties);

                    _pouchHeader.Text = result 
                        ? $"{e.Widget.Type} attached properties inherited from parent" 
                        : "Unable to show attached properties.";
                });
            });
        }


        protected override void OnUnload()
        {
            base.OnUnload();
            ToolboxApp.Bus.StopListening<ShowAttachedProperties>();
        }


        protected override Control OnDefineLayout()
        {
            _grid = new PropertyEditorGridView();

            // actionbar layout
            var actionbar = new TableLayout(1, 1)
            {
                Spacing = new Size(5, 0)
            };

            _pouchHeader = new Label();
            actionbar.Add(_pouchHeader, 0, 0, true, false);

            var layout = new TableLayout(1, 2) { Spacing = new Size(5, 5) };

            // positioning
            layout.Add(actionbar, 0, 0, true, false);
            layout.Add(_grid, 0, 1, true, true);

            layout.Shown += (sender, args) =>
            {
                ToolboxApp.AppEvents.RefreshWidget(PouchTypes.AttachedProperties);
            };

            return layout;
        }
    }
}
