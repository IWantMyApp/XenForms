using System.Diagnostics;
using XenForms.Core.Reflection;

namespace XenForms.Core.Plugin
{
    [DebuggerDisplay("{FullName}")]
    public class ViewType
    {
        public string FullName { get; set; }
        public string ShortName => ReflectionMethods.GetShortTypeName(FullName);
    }


    [DebuggerDisplay("{Type.FullName} at {DisplayPath}")]
    public class ViewRegistration
    {
        public ViewRegistration(string fullTypeName, params string[] path)
        {
            Type = new ViewType
            {
                FullName = fullTypeName,
            };

            Path = path;
        }

        public ViewType Type { get; set; }
        public string[] Path { get; set; }
        public string DisplayPath => string.Join("->", Path);
    }
}