using System;
using Eto.Drawing;

namespace XenForms.Toolbox.UI.Shell.Pouches
{
    public abstract class ToolbeltPouch : XenVisualElement
    {
        public string BeltName { get; set; }
        public int DisplayOrder { get; set; }
        public Image Image { get; set; }


        protected ToolbeltPouch(string beltName, Image image = null, int displayOrder = -1)
        {
            if (String.IsNullOrWhiteSpace(beltName)) { throw new ArgumentNullException(nameof(beltName)); }

            BeltName = beltName;
            DisplayOrder = displayOrder;
            Image = image;
        }


        public override string ToString()
        {
            return BeltName;
        }
    }
}