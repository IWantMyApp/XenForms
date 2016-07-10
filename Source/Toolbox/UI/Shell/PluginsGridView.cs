using System.Linq;
using Eto.Forms;
using Ninject;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Shell
{
    public class PluginsGridView : GridView
    {
        public PluginsGridView()
        {
            var plugins = ToolboxApp
                .Services
                .GetAll<IPluginRegistration>()
                .ToArray();

            ShowHeader = true;
            GridLines = GridLines.Horizontal;

            Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Name", DataCell = new TextBoxCell("PluginName"), Editable = false });
            Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Version", DataCell = new TextBoxCell("Version"), Editable = false });
            Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Author", DataCell = new TextBoxCell("Author"), Editable = false });
            Columns.Add(new GridColumn { AutoSize = true, HeaderText = "Web Site", DataCell = new TextBoxCell("WebSite"), Editable = false });

            if (!plugins.Any())
            {
                DataStore = new[]
                {
                    new
                    {
                        // matches the databinding id and not header text.
                        PluginName = "No plugins were found. Enable tracing, if this is an error."
                    }
                };

                ShowHeader = false;
            }
            else
            {
                DataStore = plugins;
            }
        }
    }
}