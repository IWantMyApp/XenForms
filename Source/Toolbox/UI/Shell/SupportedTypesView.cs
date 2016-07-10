using System.Linq;
using Eto.Forms;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Shell
{
    public class SupportedTypesView : XenVisualElement
    {
        protected override Control OnDefineLayout()
        {
            var notConnected = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Text = "Connect to a designer before viewing this information."
            };

            if (!ToolboxApp.SocketManager.IsConnected)
            {
                return notConnected;
            }

            var designerCol = new GridColumn
            {
                Resizable = true,
                Editable = false,
                AutoSize = true,
                HeaderText = "Full Type Name",
                DataCell = new TextBoxCell(nameof(PropertyEditorInformation.DesignTypeName))
            };

            var grid = new GridView
            {
                Columns = { designerCol },
                DataStore = ToolboxApp.Editors.PropertyEditors.OrderBy(t => t.DesignTypeName)
            };

            return grid;
        }
    }
}
