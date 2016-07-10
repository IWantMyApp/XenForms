using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Widgets;
using XenForms.Toolbox.UI.Pouches.Events.Cells;
using XenForms.Toolbox.UI.Shell.Pouches;
using XenForms.Toolbox.UI.Shell.Styling;


namespace XenForms.Toolbox.UI.Pouches.Events
{
    public class EventsPouch : ToolbeltPouch
    {
        private readonly FilterCollection<IToolboxEventModel> _filtered;
         
        private TableLayout _layout;
        private Label _pouchHeader;
        private GridView _eventsGrid;


        public EventsPouch() : base("Events")
        {
            _filtered = new FilterCollection<IToolboxEventModel>();

            // Wait for the events to be received from the toolbox and parsed before showing
            ToolboxApp.Bus.Listen<ShowWidgetEvents>(args =>
            {
                Application.Instance.AsyncInvoke(() => PopulateEvents(args.Widget));
            });

            ToolboxApp.Bus.Listen<RefreshWidgetEvent>(args =>
            {
                _eventsGrid.ReloadData(args.Row);
            });
        }


        protected override void OnUnload()
        {
            base.OnUnload();
            ToolboxApp.Bus.StopListening<ShowWidgetEvents>();
            ToolboxApp.Bus.StopListening<RefreshWidgetEvent>();
        }


        protected override Control OnDefineLayout()
        {
            _layout = new TableLayout(2,2);
            _pouchHeader = new Label();

            var actionBar = new TableLayout(2, 1)
            {
                Spacing = new Size(5, 0),
                Padding = new Padding(0, 0, 0, 5)
            };

            actionBar.Add(_pouchHeader, 0, 0, true, false);
            actionBar.Add(null, 1, 0, false, false);

            // Events Grid
            // visibility column; will show an image for protected or public.
            var visCol = new GridColumn
            {
                Sortable = false,
                HeaderText = "Visibility",
                Editable = false,
                DataCell = new TextBoxCell("Visibility")
            };

            // show event name
            var nameCol = new GridColumn
            {
                Sortable = false,
                HeaderText = "Name",
                Editable = false,
                DataCell = new TextBoxCell { Binding = Binding.Property((IToolboxEventModel m) => m.DisplayName) }
            };

            // event handler name
            var handlerCol = new GridColumn
            {
                Sortable = false,
                HeaderText = "Handler",
                Width = 120,
                Editable = false,
                DataCell = new TextBoxCell { Binding = Binding.Property((IToolboxEventModel m) => m.EventHandlerName) }
            };

            // edit event name
            var btnCol = new GridColumn
            {
                Sortable = false,
                HeaderText = "Edit Event",
                Editable = false,
                DataCell = new ButtonCell()
            };

            // menu
            var editCmd = new Command(OnEditItem);
            var compileCmd = new Command(OnAttachEvent);

            var editHandler = new ButtonMenuItem(editCmd)
            {
                Text = "Edit Event Handler",
                Enabled = false
            };

            var attachItem = new ButtonMenuItem(compileCmd)
            {
                Text = "Attach Event Handler",
                Enabled = false
            };

            var ctxMenu = new ContextMenu(editHandler, attachItem);

            // grid
            _eventsGrid = new GridView
            {
                Columns = { visCol, nameCol, handlerCol, btnCol },
                ContextMenu = ctxMenu,
                DataStore = _filtered,
                RowHeight = AppStyles.GridRowHeight,
            };

            _layout.Add(actionBar, 0, 0, false, false);
            _layout.Add(_eventsGrid, 0, 1, true, true);

            _layout.Shown += (sender, args) =>
            {
                ToolboxApp.AppEvents.RefreshWidget(PouchTypes.Events);
            };

            return _layout;
        }


        private void PopulateEvents(XenWidget widget)
        {
            if (widget.Events == null) return;
            _filtered.Clear();

            foreach (var wEvent in widget.Events.OrderBy(e => e.Name))
            {
                var model = new ToolboxEventModel
                {
                    DisplayName = wEvent.Name,
                    Visibility = ToolboxEventVisibility.Public
                };

                _filtered.Add(model);
            }

            _pouchHeader.Text = $"{widget.Type} events";
        }


        #region Event Handlers

        private void OnEditItem(object sender, EventArgs e)
        {
            //VisualStudioBridge.OpenFile(VisualStudioVersion.VS2015, ToolboxServices.AppEvents.Project.SelectedFile);
        }


        private void OnAttachEvent(object sender, EventArgs e)
        {
            /*
            var project = ToolboxServices.AppEvents.Project;
            var loader = new DesignCodeLoader();
            var compiler = new DesignCodeCompiler();

            byte[] data;
            compiler.FromFile(project.SelectedFile, out data);
            var generated = loader.GetAssemblyInformation(data);

            var b64 = Convert.ToBase64String(data);

            var loadMessage = XenMessage.Create<LoadAssemblyRequest>();
            loadMessage.AssemblyData = b64;

            ToolboxServices.SocketManager.Socket.Send(loadMessage);

            var attachMessage = XenMessage.Create<AttachEventHandlerRequest>();

            //attachMessage.EventName = item.;
            attachMessage.EventName = "Clicked";
            attachMessage.WidgetId = project.SelectedVisualTreeNode.Widget.Id;
            attachMessage.GeneratedAssembly = generated;

            ToolboxServices.SocketManager.Socket.Send(attachMessage);
            */
        }

        #endregion
    }
}