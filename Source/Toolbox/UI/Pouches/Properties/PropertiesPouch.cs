using System;
using System.Threading;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell;
using XenForms.Toolbox.UI.Shell.Images;
using XenForms.Toolbox.UI.Shell.Pouches;
using XenForms.Toolbox.UI.Shell.Styling;


namespace XenForms.Toolbox.UI.Pouches.Properties
{
    public class PropertiesPouch : ToolbeltPouch
    {
        private PropertyEditorGridView _grid;
        private SearchBox _search;
        private Label _pouchHeader;

        private VisualTreeNode SelectedNode { get; set; }


        public PropertiesPouch() : base(PropertiesPouchResource.Title)
        {
            // Wait for the toolbox to receive and parse the widget's properties before attempting to show them.
            ToolboxApp.Bus.Listen<ShowWidgetPropertyEditors>(e =>
            {
                // should the VisualTreeNodeSelected message come in first, wait.
                SpinWait.SpinUntil(() => SelectedNode != null, TimeSpan.FromSeconds(2));

                Application.Instance.AsyncInvoke(() =>
                {
                    var result = _grid.ShowEditors(SelectedNode);

                    if (result)
                    {
                        _pouchHeader.Text = $"{SelectedNode.DisplayName} properties";
                    }
                });
            });

            // listen and record the visual tree node that was selected. 
            ToolboxApp.Bus.Listen<VisualTreeNodeSelected>(args =>
            {
                SelectedNode = args.Node;
            });
        }


        protected override void OnUnload()
        {
            base.OnUnload();
            ToolboxApp.Bus.StopListening<ShowWidgetPropertyEditors>();
        }


        protected override Control OnDefineLayout()
        {
            _grid = new PropertyEditorGridView();

            var settingsBtn = new ButtonMenuList
            {
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight,
                Image = AppImages.Filter
            };

            var addTypeBtn = new ImageButton
            {
                Image = AppImages.Plus,
                ToolTip = "Register New Property Type",
                Width = AppStyles.IconWidth,
                Height = AppStyles.IconHeight
            };

            settingsBtn.AddCheckItem("Show Types", true, chkd =>
            {
                var column = _grid.Columns[_grid.TypeNameColumnIndex];
                column.Visible = chkd;
            });

            addTypeBtn.Click += OnAddSupportedType;
            
            // actionbar layout
            var actionbar = new TableLayout(3, 1)
            {
                Spacing = new Size(5, 0)
            };

            _pouchHeader = new Label();

            actionbar.Add(_pouchHeader, 0, 0, true, false);
            actionbar.Add(addTypeBtn, 1, 0, false, false);
            actionbar.Add(settingsBtn, 2, 0, false, false);

            var layout = new TableLayout(1, 3) { Spacing = new Size(5, 5) };

            // search
            _search = new SearchBox {PlaceholderText = "Search"};
            _search.TextChanged += OnSearch;

            // positioning
            layout.Add(actionbar, 0, 0, true, false);
            layout.Add(_search, 0, 1, true, false);
            layout.Add(_grid, 0, 2, true, true);

            layout.Shown += (sender, args) =>
            {
                ToolboxApp.AppEvents.RefreshWidget(PouchTypes.Properties);
            };

            return layout;
        }


        private void OnAddSupportedType(object sender, EventArgs e)
        {
            var dlg = new RegisterNewTypeDialog();
            dlg.Show();
        }


        /// <summary>
        /// Filter the visible properties by name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearch(object sender, EventArgs e)
        {
            _grid.Editors.Filter = m => m.DisplayName.IndexOf(_search.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}