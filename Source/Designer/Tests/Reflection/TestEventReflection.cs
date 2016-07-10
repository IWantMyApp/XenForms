using System;
using NUnit.Framework;
using Xamarin.Forms;
using XenForms.Core.Reflection;

namespace XenForms.Designer.Tests.Reflection
{
    [TestFixture]
    public class TestEventReflection
    {
        [Test]
        public void Get_private_events_subscribers()
        {
            var btn = new Button();
            btn.Clicked += BtnOnClicked;
            var evt = btn.GetEventSubscribers("Clicked");
        }


        private void BtnOnClicked(object sender, EventArgs eventArgs)
        {
            // do nothing
        }
    }
}
