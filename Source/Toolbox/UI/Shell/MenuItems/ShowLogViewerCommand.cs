using System;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Diagnostics;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Logging;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ShowLogViewerCommand : Command
    {
        private GridView _gridView;
        private Dialog _dialog;
        private SearchBox _control;


        public ShowLogViewerCommand()
        {
            MenuText = LoggingResource.Dialog_title;
            ToolBarText = LoggingResource.Dialog_title;
        }


        protected override async void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            
            DefineLayout();
            PopulateGridView();
            _gridView.Focus();
            
            await _dialog.ShowModalAsync(Application.Instance.MainForm);
        }


        private void DefineLayout()
        {
            var containingLayout = new TableLayout(1, 3);

            var filterLayout = new TableLayout(2, 1)
            {
                Padding = new Padding(5)
            };

            var exportLayout = new TableLayout(4, 1)
            {
                Padding = new Padding(10),
                Spacing = new Size(10, 0)
            };

            var okBtn = new Button {Text = CommonResource.Ok};
            var clearBtn = new LinkButton {Text = LoggingResource.Clear};

            _control = new SearchBox {PlaceholderText = LoggingResource.Search_logs, Width = 200};
            filterLayout.Add(null, 0, 0, true, false);
            filterLayout.Add(_control, 1, 0, false, false);

            var exportBtn = new LinkButton {Text = LoggingResource.Export_log};
            exportLayout.Add(exportBtn, 0, 0, false, false);
            exportLayout.Add(clearBtn, 1, 0, false, false);
            exportLayout.Add(null, 2, 0, true, false);
            exportLayout.Add(okBtn, 3, 0, false, false);

            exportBtn.Click += (s, e) => { SaveLogDialog.Show(); };
            clearBtn.Click += (s, e) =>
            {
                ToolboxApp.Log.ClearLiveLog();
                PopulateGridView();
            };

            _gridView = new GridView
            {
                AllowColumnReordering = false,
                ShowHeader = true,
            };

            _gridView.Columns.Add(new GridColumn { HeaderText = LoggingResource.Header_time, AutoSize = true, DataCell = new TextBoxCell { Binding = new DelegateBinding<LogItem, string>(i => FormatEventTime(i.EventTime))}, Editable = false, Sortable = false });
            _gridView.Columns.Add(new GridColumn { HeaderText = LoggingResource.Header_severity, AutoSize = true, DataCell = new TextBoxCell { Binding = new DelegateBinding<LogItem, string>(i => i.Severity)}, Editable = false, Sortable = false });
            _gridView.Columns.Add(new GridColumn { HeaderText = LoggingResource.Header_description, AutoSize = true, DataCell = new TextBoxCell { Binding = new DelegateBinding<LogItem, string>(i => i.Description)}, Editable = false, Sortable = false });


            containingLayout.Add(filterLayout, 0, 0, true, false);
            containingLayout.Add(_gridView, 0, 1, true, true);
            containingLayout.Add(exportLayout, 0, 2, false, false);

            var size = new Size(850, 400);

            _dialog = new Dialog
            {
                Icon = AppImages.Xf,
                Title = LoggingResource.Dialog_title,
                Content = containingLayout,
                ClientSize = size,
                MinimumSize = size,
                Maximizable = false,
                Resizable = true,
                AbortButton = okBtn
            };

            okBtn.Click += (sender, args) => _dialog.Close();
        }


        private void PopulateGridView()
        {
            var filtered = new SelectableFilterCollection<LogItem>(_gridView, ToolboxApp.Log.LiveLogItems);
            _gridView.DataStore = filtered;

            _control.TextChanged += (s, e) =>
            {
                var filterItems = (_control.Text ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (filterItems.Length == 0)
                {
                    filtered.Filter = null;
                }
                else
                    filtered.Filter = i =>
                    {
                        foreach (var filterItem in filterItems)
                        {
                            if (i.ToString().IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
                            {
                                return false;
                            }
                        }
                        return true;
                    };
            };
        }


        private string FormatEventTime(DateTime time)
        {
            return $"{time.ToShortDateString()} {time.ToLongTimeString()}";
        }
    }
}
