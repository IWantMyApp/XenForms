using Eto.Drawing;
using Eto.Forms;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell
{
    public static class WorkingDialog
    {
        private static bool _showing;
        private static ConnectedDialog _dlg;
        private static ProgressBar _progress;


        public static void Show()
        {
            if (_showing)
            {
                _dlg.Close();
                _showing = false;
            }

            _progress = new ProgressBar
            {
                Indeterminate = true
            };

            _dlg = new ConnectedDialog
            {
                Title = "Working...",
                Padding = new Padding(10),
                Content = _progress,
                Size = new Size(300, 80),
                Icon = AppImages.Xf
            };

            _showing = true;
            _dlg.ShowModal(Application.Instance.MainForm);
        }


        public static void Close()
        {
            if (_dlg == null) return;

            _showing = false;
            _dlg.Close();
        }
    }
}