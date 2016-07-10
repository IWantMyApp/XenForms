using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using XenForms.Core.FileSystem;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox.Actions;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Core.Toolbox.Workflow;
using XenForms.Core.Widgets;
using XenForms.Core.XAML;

namespace XenForms.Core.Toolbox
{
    public sealed class ToolboxAppEvents
    {
        [Inject]
        public IEnumerable<IPluginRegistration> Plugins { get; set; }

        [Inject]
        public PropertyEditorManager Editors { get; set; }

        [Inject]
        public IMessageBus Bus { get; set; }

        [Inject]
        public ToolboxLogging Log { get; set; }

        [Inject]
        public ToolboxSocketManager SocketManager { get; set; }

        [Inject]
        public ToolboxWorkflow Workflow { get; set; }

        [Inject]
        public IEnumerable<IStartupTask> StartupTasks { get; set; }

        [Inject]
        public ProjectWorkspace Project { get; set; }

        [Inject]
        public IXamlPostProcessor XamlProcessor { get; set; }

        [Inject]
        public IFileSystem FileSystem { get; set; }

        [Inject]
        public DesignerBridge Designer { get; set; }


        private bool _initialized;
        private PouchTypes _dataReceived = PouchTypes.None;


        /// <summary>
        /// This method should be executed before the workspae is shown.
        /// Additionally, it should only be executed once during the lifetime of the process.
        /// </summary>
        /// <param name="resolver">Inject a service locator</param>
        internal void ApplicationInitialize(Action<object> resolver)
        {
            if (_initialized) return;
            _initialized = true;

            ToolboxWorkflow.GetServices = resolver;
            SocketManager.Socket.MessageReceived += OnDesignerMessageReceived;
            RegisterToolboxActions();

            Bus.Listen<DesignerReadyEvent>(_ => OnConnectedToDesigner());
            Bus.Listen<DisconnectedFromDesigner>(_ => OnDisconnectedFromDesigner());
            Bus.Listen<NewVisualTreeRootSet>(_ => OnVisualTreeRootSet());
            
            Bus.Listen<ExecuteStartupTasks>(async _ =>
            {
                await OnExecuteStartupTasks(ExecuteWhen.ApplicationInitialized);
            });
        }


        /// <summary>
        /// Call this method when the application's process will be terminated.
        /// It should be called once during the processes' lifetime.
        /// </summary>
        public void ApplicationCleanUp()
        {
            // chosing not to unsubscribe from message bus events because the process is ending
            SocketManager.Socket.MessageReceived -= OnDesignerMessageReceived;
            SocketManager.Disconnect();
            _initialized = false;
        }


        internal void OnConnectedToDesigner()
        {
            Bus.Listen<VisualTreeNodeSelected>(OnVisualTreeNodeSelected);
            Bus.Listen<ForceVisualTreeRefresh>(_ => Designer.GetVisualTree());
            Bus.Listen<XamlElementDefaultsReceived>(e => SaveXamlElementDefaults(e.Xaml));
            Bus.Listen<OpenXamlResponseReceived>(e => OnOpenXaml(e.Xaml, e.FileName));

            Editors.Initialize();
            XamlProcessor.Reset();
            RegisterPropertyEditors();
            SendSupportedTypesMessage();
            Designer.GetVisualTree();
            Designer.GetDesignSurfaceXaml();
        }


        internal void OnDisconnectedFromDesigner()
        {
            Bus.StopListening<VisualTreeNodeSelected>();
            Bus.StopListening<ForceVisualTreeRefresh>();
            Bus.StopListening<XamlElementDefaultsReceived>();
            Bus.StopListening<OpenXamlResponseReceived>();

            VisualTree.Reset();
            Editors.Reset();
            var reset = Project.Reset();

            if (reset.AssembliesLoaded)
            {
                Bus.Notify(new RestartDesignerEvent());
            }
        }


        internal async Task OnExecuteStartupTasks(ExecuteWhen when)
        {
            var ordered = StartupTasks
                .Where(t => t.ExecuteWhen == when)
                .OrderBy(t => t.ExecutionOrder)
                .ToArray();

            if (ordered.Length == 0)
            {
                Log.Warn($"No startup tasks were found to execute during {when}.");
            }

            foreach (var task in ordered)
            {
                Log.Info($"Executing startup task {task.GetType().Name}.");
                await task.ExecuteAsync(null);
            }
        }


        internal void OnVisualTreeRootSet()
        {
            if (VisualTree.XamlElements != null) return;

            if (!string.IsNullOrWhiteSpace(Project.State.OpenFile))
            {
                try
                {
                    string xml;
                    FileSystem.ReadAllText(Project.State.OpenFile, out xml);

                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var xtr = new XamlTreeReader(xml);
                        xtr.Read();
                        VisualTree.XamlElements = xtr.All;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Couldn't load {Project.State.OpenFile}. The XAML output will not be exact.");
                }
            }
        }


        internal void OnVisualTreeNodeSelected(VisualTreeNodeSelected args)
        {
            Project.State.SelectedVisualTreeNode = args.Node;
            _dataReceived = PouchTypes.None;
            RefreshWidget(Project.State.SelectedPouch);
        }


        internal void RefreshWidget(PouchTypes pouch)
        {
            Project.State.SelectedPouch = pouch;

            if (Project.State.SelectedVisualTreeNode == null || pouch == PouchTypes.None)
            {
                return;
            }

            var node = Project.State.SelectedVisualTreeNode;

            switch (pouch)
            {
                case PouchTypes.Properties:

                    if (!_dataReceived.HasFlag(PouchTypes.Properties))
                    {
                        GetWidgetProperties(node);
                        _dataReceived |= PouchTypes.Properties;
                    }

                    break;
                case PouchTypes.AttachedProperties:

                    if (!_dataReceived.HasFlag(PouchTypes.AttachedProperties))
                    {
                        GetWidgetAttachedProperties(node);
                        _dataReceived |= PouchTypes.AttachedProperties;
                    }

                    break;
                case PouchTypes.Events:

                    if (!_dataReceived.HasFlag(PouchTypes.Events))
                    {
                        GetWidgetEvents(node);
                        _dataReceived |= PouchTypes.Events;
                    }

                    break;
            }
        }


        private void OnOpenXaml(string[] xaml, string fileName)
        {
            XamlProcessor.Reset();
            SaveXamlElementDefaults(xaml);
            Project.State.OpenFile = fileName;
        }


        private void GetWidgetProperties(VisualTreeNode node)
        {
            GetWidgetProperties(node?.Widget?.Id);
        }


        private void GetWidgetProperties(string widgetId)
        {
            if (string.IsNullOrWhiteSpace(widgetId))
            {
                Log.Error($"{nameof(GetWidgetProperties)} called with an empty {nameof(widgetId)}. Ignored.");
                return;
            }

            Designer.GetWidgetProperties(widgetId);
        }


        private void GetWidgetEvents(VisualTreeNode node)
        {
            var id = node?.Widget?.Id;

            if (string.IsNullOrWhiteSpace(id))
            {
                Log.Error($"{nameof(GetWidgetEvents)} called with an empty {nameof(node)}. Ignored.");
                return;
            }

            Designer.GetWidgetEvents(id);
        }


        private void GetWidgetAttachedProperties(VisualTreeNode node)
        {
            var id = node?.Widget?.Id;

            if (string.IsNullOrWhiteSpace(id))
            {
                Log.Error($"{nameof(GetWidgetAttachedProperties)} called with an empty {nameof(node)}. Ignored.");
                return;
            }

            Designer.GetAttachedProperties(id);
        }


        private void SaveXamlElementDefaults(string[] xaml)
        {
            try
            {
                foreach (var item in xaml)
                {
                    XamlProcessor.LoadElementDefaults(item);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }


        private void SendSupportedTypesMessage()
        {
            var types = Editors
                .PropertyEditors
                .Select(t => new XenType
                {
                    Descriptor = XenPropertyDescriptors.None,
                    FullName = t.DesignTypeName
                }).ToArray();

            Designer.SendSupportedTypes(types);
        }


        private void RegisterToolboxActions()
        {
            Log.Info("Registering default toolbox actions.");

            Workflow.Register<GetVisualTreeResponse, GetVisualTreeAction>();
            Workflow.Register<GetWidgetPropertiesResponse, GetWidgetPropertiesAction>();
            Workflow.Register<GetWidgetEventsResponse, GetWidgetEventsAction>();
            Workflow.Register<ObjectResponse, GetObjectAction>();
            Workflow.Register<EditCollectionResponse, EditCollectionAction>();
            Workflow.Register<GetConstructorsResponse, GetConstructorsAction>();
            Workflow.Register<CallConstructorResponse, CallConstructorAction>();
            Workflow.Register<DesignerReady, DesignerReadyAction>();
            Workflow.Register<GetAttachedPropertiesResponse, AttachedPropertiesReceivedAction>();
            Workflow.Register<SaveXamlResponse, SaveXamlAction>();
            Workflow.Register<CreateWidgetResponse, XamlResponseAction>();
            Workflow.Register<OpenXamlResponse, OpenXamlAction>();
            Workflow.Register<AddSupportedTypeResponse, AddSupportedTypeAction>();

            Log.Info("Finished registering default toolbox actions.");
        }


        private void RegisterPropertyEditors()
        {
            Log.Info("Registering default property editors.");

            Editors.Add(typeof(byte), typeof(byte));
            Editors.Add(typeof(byte?), typeof(byte?));

            Editors.Add(typeof(short), typeof(short));
            Editors.Add(typeof(short?), typeof(short?));

            Editors.Add(typeof(ushort), typeof(ushort));
            Editors.Add(typeof(ushort?), typeof(ushort?));

            Editors.Add(typeof(int), typeof(int));
            Editors.Add(typeof(int?), typeof(int?));

            Editors.Add(typeof(uint), typeof(uint));
            Editors.Add(typeof(uint?), typeof(uint?));

            Editors.Add(typeof(long), typeof(long));
            Editors.Add(typeof(long?), typeof(long?));

            Editors.Add(typeof(ulong), typeof(ulong));
            Editors.Add(typeof(ulong?), typeof(ulong?));

            Editors.Add(typeof(float), typeof(float));
            Editors.Add(typeof(float?), typeof(float?));

            Editors.Add(typeof(double), typeof(double));
            Editors.Add(typeof(double?), typeof(double?));

            Editors.Add(typeof(Guid), typeof(string));
            Editors.Add(typeof(string));
            Editors.Add(typeof(Enum), typeof(string));

            Log.Info("Finished registering default property editors.");

            if (Plugins != null)
            {
                foreach (var plugin in Plugins)
                {
                    Log.Info($"Registering property editors for {plugin.PluginName} {plugin.UniqueId}.");
                    plugin.Register(Editors);
                    Log.Info($"Finished registering property editors for {plugin.PluginName} {plugin.UniqueId}.");
                }
            }
        }


        private void OnDesignerMessageReceived(object sender, SocketMessageReceivedEventArgs args)
        {
            Workflow.Queue(args.Message, OnNextSuggestedMessage);
        }


        private void OnNextSuggestedMessage(XenMessage msg)
        {
            if (msg.Is<GetVisualTreeRequest>())
            {
                Designer.GetVisualTree();
            }

            if (msg.Is<GetWidgetPropertiesRequest>())
            {
                var r = msg as GetWidgetPropertiesRequest;
                GetWidgetProperties(r?.WidgetId);
            }
        }
    }
}