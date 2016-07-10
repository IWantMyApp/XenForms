using XenForms.Core.Reflection;

namespace XenForms.Core.Widgets
{
    public class XenPropertyRef : IShallowClone<XenPropertyRef>
    {
        public string PropertyName { get; set; }
        public XenPropertyRef Parent { get; set; }
        public int? EnumerableIndex { get; set; }
        public bool IsEnumerable { get; set; }


        public XenPropertyRef ShallowClone()
        {
            return new XenPropertyRef
            {
                PropertyName = PropertyName,
                Parent = Parent.ShallowClone(),
                EnumerableIndex = EnumerableIndex,
                IsEnumerable = IsEnumerable,
            };
        }
    }
}