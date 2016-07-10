using System.Diagnostics;
using System.Reflection;

namespace XenForms.Core.Widgets
{
    [DebuggerDisplay("{Name}")]
    public class XenEvent
    {
        public static XenEvent Create(EventInfo info)
        {
            var result = new XenEvent
            {
                Name = info.Name,
                IsPublic = info.AddMethod.IsPublic,
                IsFamily = info.AddMethod.IsFamily,
                IsStatic = info.AddMethod.IsStatic,
                IsFamilyOrAssembly = info.AddMethod.IsFamilyOrAssembly
            };

            return result;
        }

        public string Signature { get; set; }
        public string Name { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFamily { get; set; }
        public bool IsFamilyOrAssembly { get; set; }
        public bool IsPublic { get; set; }
        public int Subscribers { get; set; }
    }
}