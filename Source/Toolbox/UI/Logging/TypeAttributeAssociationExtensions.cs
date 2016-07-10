using System;
using XenForms.Core.Diagnostics;
using XenForms.Core.Reflection;
using XenForms.Core.Toolbox;

namespace XenForms.Toolbox.UI.Logging
{
    public static class TypeAttributeAssociationExtensions
    {
        public static void LogDiagnostics<T>(this TypeAttributeAssociation<T>[] associations, XenLogLevel level) where T : Attribute
        {
            foreach (var association in associations)
            {
                var message = $"Found {association.DecoratedType.FullName}";
                ToolboxApp.Log.Event(level, message);
            }
        }
    }
}