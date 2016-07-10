using System;
using Newtonsoft.Json;
using XenForms.Core.Messages;
using XenForms.Core.Toolbox.AppEvents;
using XenForms.Core.Widgets;

namespace XenForms.Core.Toolbox.Actions
{
    public class GetObjectAction : ToolboxAction
    {
        protected override void OnExecute(XenMessage message)
        {
            if (message.Is<ObjectResponse>())
            {
                HandleObject(message as ObjectResponse);
            }
        }


        private void HandleObject(ObjectResponse response)
        {
            var json = response.Property?.Value?.ToString();
            var properties = Convert(json);

            // todo: change to membus?
            var oArgs = new ObjectPropertiesReceived(properties, response.WidgetId, response.ObjectName);
            var cArgs = new CollectionPropertiesReceived(properties, response.WidgetId, response.ObjectName);

            Bus.Notify(oArgs);
            Bus.Notify(cArgs);
        }


        private XenProperty[] Convert(string json)
        {
            XenProperty[] properties = null;

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    properties = JsonConvert.DeserializeObject<XenProperty[]>(json);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return properties;
        }
    }
}
