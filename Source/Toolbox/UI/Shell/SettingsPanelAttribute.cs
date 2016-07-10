using System;

namespace XenForms.Toolbox.UI.Shell
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsPanelAttribute : Attribute
    {
        
        public sealed class Builtin
        {
            public const string General = "General";
            public const string Environment = "Environment";
        }


        public string GroupName { get; set; }
        public string PanelName { get; set; }


        public SettingsPanelAttribute(string groupName, string panelName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) { throw new ArgumentNullException(nameof(groupName)); }
            if (string.IsNullOrWhiteSpace(panelName)) { throw new ArgumentNullException(nameof(panelName)); }

            GroupName = groupName;
            PanelName = panelName;
        }
    }
}
