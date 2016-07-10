using System;
using System.Threading;
using Xamarin.Forms;
using XenForms.Core.Designer;

namespace XenForms.Designer.XamarinForms.UI
{
    public class XamarinDesignThread : IDesignThread
    {
        public object Context { get; set; }


        public void Invoke(Action action)
        {
            var page = Context as ContentPage;
            if (page == null) return;
            var done = false;

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action();

                    // refresh content
                    var content = page.Content;
                    page.Content = null;
                    page.Content = content;

                    done = true;
                }
                catch (Exception)
                {
                    App.CurrentDesignSurface.DisplayAlert("Design Error #1000",
                        "An error occurred. See the toolbox log.",
                        "Ok");
                }
            });

            SpinWait.SpinUntil(() => done, TimeSpan.FromSeconds(3));
        }
    }
}