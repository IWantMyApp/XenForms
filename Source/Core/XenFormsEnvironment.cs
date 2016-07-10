using System.Reflection;

namespace XenForms.Core
{
    public abstract class XenFormsEnvironment
    {
        public const int DesignerVersion = 3;
        public const string ProjectFileSchema = "https://schemas.xenforms.com/xenproject/v1/";
        public static string ToolboxVersion { get; } = typeof(XenFormsEnvironment).GetTypeInfo().Assembly.GetName().Version.ToString();

        public abstract string GetOsInformation();
        public abstract string GetMemoryUsage();
    }
}