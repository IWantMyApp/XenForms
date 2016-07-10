using Eto.Drawing;
using XenForms.Core.Plugin;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Plugins.XamarinForms
{
    public class PluginRegistration : IPluginRegistration
    {
        private static readonly string[] ViewPath = {"New", "View"};
        private static readonly string[] LayoutPath = {"New", "Layout"};


        public string PluginName { get; } = "Xamarin.Forms 2.3.0.45";
        public string Version { get; } = "1.0.0";
        public string Author { get; } = "Michael Davis";
        public string WebSite { get; } = "https://www.github.com/michaeled/xenforms/";
        public string AutoUpdateUrl { get; } = "https://plugins.xenforms.com/official/xamarinforms/autoupdate.xml";
        public string UniqueId { get; } = "{80f4ca8b-d04b-48c4-89c1-f6c184fa3b00}";


        public ViewRegistration[] Views { get; } =
        {
            new ViewRegistration("Xamarin.Forms.ActivityIndicator", ViewPath),
            new ViewRegistration("Xamarin.Forms.BoxView", ViewPath),
            new ViewRegistration("Xamarin.Forms.Button", ViewPath),
            new ViewRegistration("Xamarin.Forms.DatePicker", ViewPath),
            new ViewRegistration("Xamarin.Forms.Editor", ViewPath),
            new ViewRegistration("Xamarin.Forms.Entry", ViewPath),
            new ViewRegistration("Xamarin.Forms.Image", ViewPath),
            new ViewRegistration("Xamarin.Forms.Label", ViewPath),
            new ViewRegistration("Xamarin.Forms.ListView", ViewPath),
            new ViewRegistration("Xamarin.Forms.Picker", ViewPath),
            new ViewRegistration("Xamarin.Forms.ProgressBar", ViewPath),
            new ViewRegistration("Xamarin.Forms.SearchBar", ViewPath),
            new ViewRegistration("Xamarin.Forms.Slider", ViewPath),
            new ViewRegistration("Xamarin.Forms.Switch", ViewPath),
            new ViewRegistration("Xamarin.Forms.TableView", ViewPath),
            new ViewRegistration("Xamarin.Forms.TimePicker", ViewPath),
            new ViewRegistration("Xamarin.Forms.WebView", ViewPath),
            new ViewRegistration("Xamarin.Forms.ContentView", LayoutPath),
            new ViewRegistration("Xamarin.Forms.ScrollView", LayoutPath),
            new ViewRegistration("Xamarin.Forms.Frame", LayoutPath),
        };


        public void Register(PropertyEditorManager manager)
        {
            manager.Add("Xamarin.Forms.BindableProperty", typeof(object));
            manager.Add("Xamarin.Forms.Color", typeof(Color));
            manager.Add("Xamarin.Forms.ImageSource", typeof(string));
            manager.Add("Xamarin.Forms.Size", typeof(object));
            manager.Add("Xamarin.Forms.Rectangle", typeof(object));
            manager.Add("Xamarin.Forms.Font", typeof(object));
            manager.Add("Xamarin.Forms.LayoutOptions", typeof(object));
            manager.Add("Xamarin.Forms.GridLength", typeof(object));
            manager.Add("Xamarin.Forms.Point", typeof(object));
            manager.Add("Xamarin.Forms.SizeRequest", typeof(object));
            manager.Add("Xamarin.Forms.Thickness", typeof(object));
            manager.Add("Xamarin.Forms.Vec2", typeof(object));
            manager.Add("Xamarin.Forms.ColumnDefinitionCollection", typeof(object));
            manager.Add("Xamarin.Forms.RowDefinitionCollection", typeof(object));
            manager.Add("Xamarin.Forms.RowDefinition", typeof(object));
        }
    }
}