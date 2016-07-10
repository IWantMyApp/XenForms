using Xamarin.Forms;
using XenForms.Core.Messages;
using XenForms.Core.Networking;

namespace XenForms.Designer.XamarinForms.UI.Pages
{
    public class DesignSurfacePage : ContentPage
    {
        private readonly DesignServer _server;


        public DesignSurfacePage(){}
        public DesignSurfacePage(DesignServer server)
        {
            _server = server;
            Title = "Design Surface";
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            var msg = XenMessage.Create<DesignerReady>();
            _server.Send(msg.ToJson());
        }
    }
}
