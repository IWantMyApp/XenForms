using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using XenForms.Core.Networking;
using XenForms.Core.Toolbox;
using XenForms.Toolbox.UI.Resources;
using XenForms.Toolbox.UI.Shell.Images;

namespace XenForms.Toolbox.UI.Shell.MenuItems
{
    public class ConnectCommand : Command
    {
        private readonly ConnectCommandModel _model;

        private Dialog _dialog;

        private TextBox _hostText;
        private TextBox _portText;
        private TextBox _keyText;

        private Button _connectBtn;
        private Button _cancelBtn;
        private TextArea _logTextArea;
        private TextBox _previewText;

        private const int FirstColumnWidth = 70;
        private const int Spacing = 10;
        private const int LogAreaHeight = 150;


        public ConnectCommand(ConnectCommandModel model)
        {
            _model = model;

            MenuText = $"{ConnectResource.Dialog_title}...";
            ToolTip = ConnectResource.Dialog_tooltip;
            Shortcut = Application.Instance.CommonModifier | Keys.T;

            ToolBarText = null;
            Image = AppImages.Connected;
        }


        public override MenuItem CreateMenuItem()
        {
            var item = base.CreateMenuItem();

            item.Validate += (sender, args) =>
            {
                item.Enabled = !ToolboxApp.SocketManager.IsConnected;
            };

            return item;
        }


        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            DefineLayout();
            _dialog.ShowModal(Application.Instance.MainForm);
        }


        private void DefineLayout()
        {
            var hostLabel = new Label
            {
                Text = ConnectResource.Host_label,
                Width = FirstColumnWidth,
                VerticalAlignment = VerticalAlignment.Center
            };

            var keyLabel = new Label
            {
                Text = ConnectResource.Key_label,
                Width = FirstColumnWidth,
                VerticalAlignment = VerticalAlignment.Center
            };

            var portLabel = new Label
            {
                Text = ConnectResource.Port_label,
                VerticalAlignment = VerticalAlignment.Center
            };

            var previewLabel = new Label
            {
                Text = "Preview",
                Width = FirstColumnWidth,
                VerticalAlignment = VerticalAlignment.Center
            };

            _previewText = new TextBox
            {
                ReadOnly = true,
            };

            _hostText = new TextBox {Width = 150};
            _portText = new TextBox {Width = 55, Text = ServiceEndpoint.Port.ToString()};
            _keyText = new TextBox {PlaceholderText = ConnectResource.Key_placeholder };

            var saveChk = new CheckBox {Text = ConnectResource.Save_label};
            _connectBtn = new Button {Text = "Connect"};
            _cancelBtn = new Button {Text = CommonResource.Cancel};

            _dialog = new Dialog
            {
                DisplayMode = DialogDisplayMode.Attached,
                Title = ConnectResource.Connect_to_host,
                DefaultButton = _connectBtn,
                AbortButton = _cancelBtn,
                Padding = new Padding(Spacing),
                ShowInTaskbar = false,
                Icon = AppImages.Xf
            };

            var mainContainer = new TableLayout(1, 5)
            {
                Spacing = new Size(0, Spacing)
            };

            var hostContainer = new TableLayout(4, 1)
            {
                Spacing = new Size(Spacing, 0)
            };

            var keyContainer = new TableLayout(2, 1)
            {
                Spacing = new Size(Spacing, 0)
            };

            var previewContainer = new TableLayout(2, 1)
            {
                Spacing = new Size(Spacing, 0)
            };

            var buttonContainer = new TableLayout(3, 1)
            {
                Spacing = new Size(5, 0)
            };

            _logTextArea = new TextArea
            {
                Height = LogAreaHeight,
                ReadOnly = true,
                Wrap = true,
            };

            var expander = new Expander
            {
                Padding = new Padding(0),
                Expanded = true,
                Header = new Label
                {
                    Text = "Status",
                    Enabled = false,
                    TextColor = Colors.Black
                },

                Content = new Panel
                {
                    Padding = new Padding(0,Spacing,0,0),
                    Content = _logTextArea
                },
            };

            expander.ExpandedChanged += (s, e) =>
            {
                if (expander.Expanded)
                {
                    _dialog.Height += LogAreaHeight;
                }
                else
                {
                    _dialog.Height -= expander.Content.Height;
                }
            };

            hostContainer.Add(hostLabel, 0, 0, false, false);
            hostContainer.Add(_hostText, 1, 0, false, false);
            hostContainer.Add(portLabel, 2, 0, false, false);
            hostContainer.Add(_portText, 3, 0, false, false);

            keyContainer.Add(keyLabel, 0, 0, false, false);
            keyContainer.Add(_keyText, 1, 0, true, false);

            previewContainer.Add(previewLabel, 0, 0, false, false);
            previewContainer.Add(_previewText, 1, 0, true, false);

            buttonContainer.Add(saveChk, 0, 0, true, false);
            buttonContainer.Add(_cancelBtn, 1, 0, false, false);
            buttonContainer.Add(_connectBtn, 2, 0, false, false);

            mainContainer.Add(hostContainer, 0, 0, true, false);
            mainContainer.Add(previewContainer, 0, 1, true, false);
            mainContainer.Add(keyContainer, 0, 2, true, false);
            mainContainer.Add(buttonContainer, 0, 3, true, false);
            mainContainer.Add(expander, 0, 4, true, true);

            _dialog.DataContext = _model;
            _dialog.Content = mainContainer;

            _dialog.PreLoad += (s, e) => OnPreload();
            _dialog.Closing += OnDialogClosing;

            _connectBtn.Bind(b => b.Enabled, _model, v => v.OkEnabled);
            _hostText.Bind(b => b.Text, _model, v => v.Host);
            _portText.Bind(b => b.Text, _model, v => v.Port);

            saveChk.Bind(b => b.Checked, _model, v => v.RememberSettings);
            _dialog.LoadComplete += (s, e) => _connectBtn.Focus();

            _connectBtn.Click += OnConnectBtnClicked;
            _cancelBtn.Click += OnCancelBtnClicked;

            _hostText.KeyUp += (s, e) => SetPreviewText();
            _portText.KeyUp += (s, e) => SetPreviewText();
        }


        private void SetPreviewText()
        {
            _previewText.Text = String.Empty;

            if (!ServiceEndpoint.IsValidPort(_portText.Text))
            {
                return;
            }

            var port = short.Parse(_portText.Text);
            _previewText.Text = ServiceEndpoint.FormatAddress(_hostText.Text, port, ServiceEndpoint.DesignerPath);
        }


        private void OnPreload()
        {
            _model.Restore();
            SetPreviewText();
        }


        private void OnCancelBtnClicked(object sender, EventArgs e)
        {
            _dialog.Close();
        }


        private void OnConnectBtnClicked(object sender, EventArgs e)
        {
            if (ToolboxApp.SocketManager.IsConnected)
            {
                var disconnect = DisconnectCommand.ShowPrompt();
                if (disconnect == DialogResult.Yes)
                {
                    ToolboxApp.SocketManager.Disconnect();
                }

                return;
            }

            var port = short.Parse(_model.Port);

            ToolboxApp.SocketManager.Connect(_hostText.Text, port, OnConnect, OnTrace);
        }


        private void OnTrace(string msg)
        {
            try
            {
                Application.Instance.Invoke(() =>
                {
                    _logTextArea.Text += $"{msg}{Environment.NewLine}";
                });
            }
            catch (Exception)
            {
                // ignored; this can occur when closing the main form while writing to the log area.
            }
        }


        private void OnConnect()
        {
            if (ToolboxApp.IsTerminating) return;

            _model.Save();
            _dialog.Close();
        }


        private void OnDialogClosing(object sender, CancelEventArgs e)
        {
            if (!ToolboxApp.SocketManager.IsConnected)
            {
                ToolboxApp.SocketManager.ClearTraces();
            }
        }
    }
}