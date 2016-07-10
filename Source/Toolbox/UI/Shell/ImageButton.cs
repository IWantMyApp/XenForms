using System;
using Eto.Drawing;
using Eto.Forms;

namespace XenForms.Toolbox.UI.Shell
{
    public class ImageButton : ImageView
    {
        private Color TransparentColor { get; } = new Color(0, 0, 0, 0);
        public event EventHandler<EventArgs> Click;
        
         
        public ImageButton()
        {
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseUp += (s, e) => OnClick();
        }


        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundColor = TransparentColor;
        }


        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var highlight = SystemColors.Highlight;
            highlight.A = .7f;

            BackgroundColor = highlight;
        }


        protected virtual void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
