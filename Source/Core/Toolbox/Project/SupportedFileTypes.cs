using System.Linq;

namespace XenForms.Core.Toolbox.Project
{
    public sealed class SupportedFileTypes
    {
        public const string XenProjectFileName = "project.xen.json";
        public const string XenXamlExtension = ".xen.xaml";
        public const string XamlExtension = ".xaml";
        public const string CSharpExtension = ".cs";
        public const string AssemblyExtension = ".dll";


        public static readonly string[] SupportedExtensions =
        {
            XenXamlExtension,
            XamlExtension,
            CSharpExtension,
            AssemblyExtension
        };


        public bool IsEditable(string fileName)
        {
            var supported = IsSupported(fileName);
            if (!supported) return false;

            return !fileName.EndsWith(AssemblyExtension);
        }


        public bool IsXaml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var fn = fileName.ToLower();

            return fn.EndsWith(XenXamlExtension) || fn.EndsWith(XamlExtension);
        }


        public bool IsSupported(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var found = SupportedExtensions.Any(fileName.EndsWith);
            return found;
        }
    }
}