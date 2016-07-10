using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using XenForms.Core.Plugin;

namespace XenForms.Core.Widgets
{
    [DebuggerDisplay("{Type} has {Children.Count} children.")]
    public class XenWidget
    {
        public XenWidget()
        {
            Children = new List<XenWidget>();
            Properties = new List<XenProperty>();
            Events = new List<XenEvent>();
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string FullTypeName { get; set; }
        public string Name { get; set; }

        public bool CanDelete { get; set; }
        public bool IsLayout { get; set; }

        public bool AllowsManyChildren { get; set; }
        public bool HasContentProperty { get; set; }
        public bool IsContentPropertyViewType { get; set; }
        public string ContentPropertyTypeName { get; set; }

        public IList<XenWidget> Children { get; set; }
        public IList<XenProperty> Properties { get; set; }
        public IList<XenProperty> AttachedProperties { get; set; }
        public IList<XenEvent> Events { get; set; }

        [JsonIgnore]
        public XenWidget Parent { get; set; }



        public bool CanAttach(ViewRegistration registration)
        {
            var viewName = registration?.Type?.FullName;
            return CanAttach(viewName);
        }


        public bool CanAttach(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName)) return false;
            if (IsLayout) return true;
            if (HasContentProperty && IsContentPropertyViewType) return true;

            return false;
        }


        public bool CanAttach(XenWidget widget)
        {
            if (widget == null) return false;
            if (IsLayout) return true;
            if (HasContentProperty && widget.IsContentPropertyViewType) return true;
            if (HasContentProperty && widget.ContentPropertyTypeName != null && ContentPropertyTypeName == widget.ContentPropertyTypeName) return true;

            return false;
        }
    }
}