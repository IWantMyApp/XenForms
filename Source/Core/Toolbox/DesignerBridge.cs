using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox
{
    public class DesignerBridge
    {
        private readonly ToolboxSocket _socket;


        public DesignerBridge(ToolboxSocket socket)
        {
            _socket = socket;
        }


        public void SetProperty(XenWidget widget, XenProperty property, object value, bool isBase64 = false, bool isAp = false, object metadata = null)
        {
            var r = XenMessage.Create<SetPropertyRequest>();

            r.Metadata = metadata;
            r.Path = property.Path;
            r.Value = value;
            r.WidgetId = widget.Id;
            r.IsBase64 = isBase64;
            r.IsAttachedProperty = isAp;

            _socket.Send(r);
        }


        public void OpenXaml(string xaml, string selectedFile)
        {
            if (string.IsNullOrWhiteSpace(xaml)) return;
            var r = XenMessage.Create<OpenXamlRequest>();

            r.Xaml = xaml;
            r.FileName = selectedFile;

            _socket.Send(r);
        }


        public void CallConstructor(string widgetId, XenProperty property, XenConstructor ctor)
        {
            if (ctor == null) return;
            var r = XenMessage.Create<CallConstructorRequest>();

            r.Constructor = ctor;
            r.Property = property;
            r.WidgetId = widgetId;

            _socket.Send(r);
        }


        public void GetConstructors(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return;
            var r = XenMessage.Create<GetConstructorsRequest>();

            r.TypeName = typeName;
            _socket.Send(r);
        }


        public void SaveXaml()
        {
            var r = XenMessage.Create<SaveXamlRequest>();
            _socket.Send(r);
        }


        public void NewPage()
        {
            var r = XenMessage.Create<NewPageRequest>();
            _socket.Send(r);
        }


        public void EditCollection(string widgetId, EditCollectionType type, params string[] path)
        {
            var r = XenMessage.Create<EditCollectionRequest>();

            r.WidgetId = widgetId;
            r.Type = type;
            r.Path = path;

            _socket.Send(r);
        }


        public void GetObjectProperties(string widgetId, params string[] path)
        {
            var r = XenMessage.Create<GetObjectRequest>();

            r.WidgetId = widgetId;
            r.Path = path;

            _socket.Send(r);
        }


        public void AddSupportedType(string typeName)
        {
            var r = XenMessage.Create<AddSupportedTypeRequest>();

            r.Type = new XenType
            {
                FullName = typeName,
                Descriptor = XenPropertyDescriptors.ValueType
            };

            _socket.Send(r);
        }


        public void GetWidgetProperties(string widgetId)
        {
            var r = XenMessage.Create<GetWidgetPropertiesRequest>();

            r.WidgetId = widgetId;
            r.IncludeValues = true;

            _socket.Send(r);
        }


        public void GetWidgetEvents(string widgetId)
        {
            var r = XenMessage.Create<GetWidgetEventsRequest>();
            r.WidgetId = widgetId;
            _socket.Send(r);
        }


        public void GetAttachedProperties(string widgetId)
        {
            var r = XenMessage.Create<GetAttachedPropertiesRequest>();
            r.WidgetId = widgetId;
            _socket.Send(r);
        }


        public void SendSupportedTypes(XenType[] types)
        {
            var r = XenMessage.Create<SupportedTypesRequest>();
            r.Types = types;
            _socket.Send(r);
        }


        public void GetVisualTree()
        {
            var r = XenMessage.Create<GetVisualTreeRequest>();
            _socket.Send(r);
        }


        public void GetDesignSurfaceXaml()
        {
            var r = XenMessage.Create<GetDesignSurfaceXamlRequest>();
            _socket.Send(r);
        }


        public void CreateWidget(string parentId, string typeName)
        {
            var r = XenMessage.Create<CreateWidgetRequest>();
            r.ParentId = parentId;
            r.TypeName = typeName;
            _socket.Send(r);
        }
    }
}
