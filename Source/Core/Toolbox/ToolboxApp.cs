using System.Collections.Generic;
using Ninject;
using XenForms.Core.FileSystem;
using XenForms.Core.Networking;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox.Project;
using XenForms.Core.Toolbox.Shell;
using XenForms.Core.Toolbox.Workflow;
using XenForms.Core.XAML;

namespace XenForms.Core.Toolbox
{
    public static partial class ToolboxApp
    {
        internal static IKernel Services { get; set; }
        internal static bool IsTerminating { get; set; }


        private static IEnumerable<IPluginRegistration> _plugins;
        public static IEnumerable<IPluginRegistration> Plugins
        {
            get
            {
                if (_plugins != null) return _plugins;
                return _plugins = Services.Get<IEnumerable<IPluginRegistration>>();
            }
        }


        private static DesignerBridge _designer;
        public static DesignerBridge Designer
        {
            get
            {
                if (_designer != null) return _designer;
                return _designer = Services.Get<DesignerBridge>();
            }
        }


        private static ProjectWorkspace _project;
        public static ProjectWorkspace Project
        {
            get
            {
                if (_fileSystem != null) return _project;
                return _project = Services.Get<ProjectWorkspace>();
            }
        }


        private static IFileSystem _fileSystem;
        public static IFileSystem FileSystem
        {
            get
            {
                if (_fileSystem != null) return _fileSystem;
                return _fileSystem = Services.Get<IFileSystem>();
            }
        }


        private static ISettingsStore _settings;
        public static ISettingsStore Settings
        {
            get
            {
                if (_settings != null) return _settings;
                return _settings = Services.Get<ISettingsStore>();
            }
        }


        private static ToolboxAppEvents _appEvents;
        public static ToolboxAppEvents AppEvents
        {
            get
            {
                if (_appEvents != null) return _appEvents;
                return _appEvents = Services.Get<ToolboxAppEvents>();
            }
        }


        private static PropertyEditorManager _editors;
        public static PropertyEditorManager Editors
        {
            get
            {
                if (_editors != null) return _editors;
                return _editors = Services.Get<PropertyEditorManager>();
            }
        }


        private static IMessageBus _bus;
        public static IMessageBus Bus
        {
            get
            {
                if (_bus != null) return _bus;
                return _bus = Services.Get<IMessageBus>();
            }
        }


        private static ToolboxLogging _log;
        public static ToolboxLogging Log
        {
            get
            {
                if (_log != null) return _log;
                return _log = Services.Get<ToolboxLogging>();
            }
        }


        private static ToolboxWorkflow _workflow;
        public static ToolboxWorkflow Workflow
        {
            get
            {
                if (_workflow != null) return _workflow;
                return _workflow = Services.Get<ToolboxWorkflow>();
            }
        }


        private static ToolboxSocketManager _socketManager;
        public static ToolboxSocketManager SocketManager
        {
            get
            {
                if (_socketManager != null) return _socketManager;
                return _socketManager = Services.Get<ToolboxSocketManager>();
            }
        }


        private static IXamlPostProcessor _xamlProcessor;
        public static IXamlPostProcessor XamlProcessor
        {
            get
            {
                if (_xamlProcessor != null) return _xamlProcessor;
                return _xamlProcessor = Services.Get<IXamlPostProcessor>();
            }
        }
    }
}