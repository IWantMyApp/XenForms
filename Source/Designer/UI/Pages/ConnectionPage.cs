using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;
using XenForms.Core.XAML;

namespace XenForms.Designer.XamarinForms.UI.Pages
{
    public class ConnectionPage : ContentPage
    {
        private readonly DesignServer _server;
        private readonly IXamlLoader _xamlLoader;
        private readonly ITypeFinder _typeFinder;
        private readonly IGetDesignerVersion _version;

        private readonly Label _status;
        private readonly Button _disconnectBtn;
        private readonly Button _continueBtn;
        private readonly Label _acceptingConnections;
        private readonly StackLayout _mainLayout;
        private readonly Label _toolboxConnected;


        public ConnectionPage(DesignServer server, IXamlLoader xamlLoader, ITypeFinder typeFinder, IGetDesignerVersion version)
        {
            _server = server;
            _xamlLoader = xamlLoader;
            _typeFinder = typeFinder;
            _version = version;

            _mainLayout = new StackLayout
            {
                Padding = new Thickness(10),
                Spacing = 5,
                Orientation = StackOrientation.Vertical
            };

            _disconnectBtn = new Button
            {
                Text = "Disconnect",
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
            };

            _continueBtn = new Button
            {
                Text = "Design Surface",
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
            };

            _acceptingConnections = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                Text = $"Ready!\nNow accepting connections to:\n{_server.ListeningOn}",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            _status = new Label
            {
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "Listening..."
            };

            _toolboxConnected = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "A toolbox is connected.\nYou can close the connection or continue to the design surface.",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            _continueBtn.Clicked += async (s, e) => await ShowDesignSurfaceAsync();
            _disconnectBtn.Clicked += (s, e) => OnDisconnect();

            RegisterServerEventHandlers();

            Content = _mainLayout;

            if (Device.OS == TargetPlatform.Android)
            {
                BackgroundImage = "background.png";
            }
            else
            {
                BackgroundColor = Color.White;
                _acceptingConnections.TextColor = Color.Black;
                _status.TextColor = Color.Black;
                _toolboxConnected.TextColor = Color.Black;
            }

            StartAcceptingConnections();
        }


        public void ShowError(string error = null)
        {
            var message = "An error occurred. See the Toolbox log.";

            if (!string.IsNullOrWhiteSpace(error))
            {
                message = error;
            }

            DisplayAlert("Design Error #1001", message, "Ok");
        }


        public void ShowError(XamlParseErrorInfo info)
        {
            DisplayAlert("Design Error #1002", info.Message, "Ok");
        }


        public void ShowError(Exception info)
        {
            DisplayAlert("Design Error #1003", $"{info}. See the Toolbox log.", "Ok");
        }


        public void PushNewDesignSurface()
        {
            var tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var page = CreateDesignSurface();

                    SetPageId(page);
                    await Navigation.PopAsync(false);
                    await Navigation.PushAsync(page, false);
                }
                catch (Exception)
                {
                    ShowError();
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            tcs.Task.Wait();
        }


        public void PushNewPage(string xaml)
        {
            var tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    Page page;

                    var discovery = new XamlDocumentDiscovery(xaml);
                    var xClassName = discovery.GetPageClassName();

                    if (string.IsNullOrWhiteSpace(xClassName))
                    {
                        page = new ContentPage();
                        var loaded = _xamlLoader.Load(page, xaml, ShowError);
                        if (loaded == null) return;
                    }
                    else
                    {
                        var type = _typeFinder.Find(xClassName);
                        if (type == null)
                        {
                            ShowError($"The type {xClassName} was not found. Try loading the project assemblies, first.");
                            return;
                        }

                        try
                        {
                            page = Activator.CreateInstance(type) as Page;
                        }
                        catch (Exception e)
                        {
                            ShowError(e);
                            return;
                        }
                    }

                    await Navigation.PopAsync(false);
                    await Navigation.PushAsync(page, false);

                    App.CurrentDesignSurface = page;
                    SetPageId(page);

                    DesignerAppEvents.SetupDesignSurface(page);
                }
                catch (Exception)
                {
                    ShowError();
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            tcs.Task.Wait();
        }


        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                var version = _version.GetVersion();
                Title = $"Connection Manager v{version}";

                if (_server.IsListening && _server.IsToolboxConnected)
                {
                    ShowAlreadyConnected();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        private void SetPageId(Page page)
        {
            if (page == null) return;
            var id = NavigationPage.GetBackButtonTitle(page);
            if (string.IsNullOrWhiteSpace(id))
            {
                NavigationPage.SetBackButtonTitle(page, App.XenClassId);
            }
        }


        private void StartAcceptingConnections()
        {
            if (!_server.IsListening)
            {
                _server.Start();
            }

            ShowAcceptingConnections();
        }
        

        private void ShowAlreadyConnected()
        {
            _mainLayout.Children.Clear();
            _mainLayout.Children.Add(_toolboxConnected);

            if (_server.IsToolboxConnected)
            {
                _mainLayout.Children.Add(new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "Connected toolbox:"
                });
            }

            var connectedToolboxes = new Label
            {
                Text = _server.ConnectedToolbox,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Start
            };

            var surfacesLoaded = new Label
            {
                Text = $"{Navigation.NavigationStack.Count - 1} surface(s) loaded.",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Start
            };

            _mainLayout.Children.Add(connectedToolboxes);
            _mainLayout.Children.Add(surfacesLoaded);

            _mainLayout.Children.Add(_continueBtn);
            _mainLayout.Children.Add(_disconnectBtn);
        }


        private void ShowAcceptingConnections()
        {
            _mainLayout.Children.Clear();
            _status.Text = "Listening...";
            _mainLayout.Children.Add(_acceptingConnections);
            _mainLayout.Children.Add(_status);
        }


        private async Task ShowDesignSurfaceAsync()
        {
            if (App.CurrentDesignSurface == null)
            {
                CreateDesignSurface();
            }

            await Navigation.PushAsync(App.CurrentDesignSurface, false);
        }


        private Page CreateDesignSurface()
        {
            App.CurrentDesignSurface = new DesignSurfacePage(_server);
            DesignerAppEvents.SetupDesignSurface(App.CurrentDesignSurface);
            return App.CurrentDesignSurface;
        }


        private void OnDisconnect()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _server.Stop();
                Reaction.Reset();

                await Navigation.PopToRootAsync(false);
                App.CurrentDesignSurface = null;

                StartAcceptingConnections();
            });
        }


        private void OnConnected()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _status.Text = "Desktop client connected...";
                await Task.Delay(200);
                _status.Text = "Opening design surface...";
                await Task.Delay(200);
                await ShowDesignSurfaceAsync();
            });
        }


        private void RegisterServerEventHandlers()
        {
            _server.ToolboxConnected += (sender, args) => OnConnected();
            _server.ToolboxDisconnected += (sender, args) => OnDisconnect();
        }
    }
}
