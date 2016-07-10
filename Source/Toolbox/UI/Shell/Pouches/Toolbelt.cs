using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Shell.Styling;

namespace XenForms.Toolbox.UI.Shell.Pouches
{
    public class Toolbelt : XenVisualElement, IEnumerable<ToolbeltPouch>
    {
        private TabControl _tabControl;
        private readonly List<ToolbeltPouch> _pouches;


        public Toolbelt() : this(new ToolbeltPouch[] {}) { }


        public Toolbelt(IEnumerable<ToolbeltPouch> pouches)
        {
            _pouches = new List<ToolbeltPouch>();

            foreach (var pouch in pouches)
            {
                Add(pouch);
            }
        }


        public void Add(ToolbeltPouch pouch)
        {
            if (pouch == null) { throw new ArgumentNullException(nameof(pouch)); }

            if (_pouches.Contains(pouch))
            {
                return;
            }

            ToolboxUI.Activate(pouch);
            _pouches.Add(pouch);
        }


        protected override Control OnDefineLayout()
        {
            _tabControl = new TabControl();
            return _tabControl;
        }


        protected override void OnBeforeLayoutShown()
        {
            base.OnBeforeLayoutShown();

            var order = _pouches.OrderBy(p => p.DisplayOrder);

            foreach (var pouch in order)
            {
                var tab = new TabPage
                {
                    Image = pouch.Image,
                    Content = pouch.View,
                    Text = pouch.BeltName,
                    Padding = AppStyles.PanelPadding
                };

                _tabControl.Pages.Add(tab);
                ToolboxApp.Log.Trace("{0} has been added to {1}.", pouch, GetType().Name);
            }
        }


        public IEnumerator<ToolbeltPouch> GetEnumerator()
        {
            return _pouches.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}