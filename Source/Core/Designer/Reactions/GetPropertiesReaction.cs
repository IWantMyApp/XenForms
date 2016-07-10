using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;

namespace XenForms.Core.Designer.Reactions
{
    /// <summary>
    /// Common logic to build <see cref="XenProperty"/> values.
    /// These properties are requested by the toolbox when type editors should be shown.
    /// </summary>
    /// <typeparam name="TVisualElement">A type common to all child widgets on the page.</typeparam>
    public abstract class GetPropertiesReaction<TVisualElement> : DesignerReaction<TVisualElement>
    {
        public XenProperty[] GetXenProperties(XenReflectionProperty[] refProps, bool includeValues = false)
        {
            if (refProps == null) return null;
            if (SupportingTypes.Types == null) return null;

            var result = new List<XenProperty>();

            // The properties are in an intermediate state, right here. 
            // a XenReflectionProperty is not meant to be returned in a request.

            // build xen properties.
            foreach (var curRef in refProps)
            {
                if (curRef.IsTargetEnum && SupportingTypes.IsRegistered(typeof(Enum)))
                {
                    if (!SupportingTypes.IsRegistered(curRef, RegistrarMatches.TypeName | RegistrarMatches.Enum))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!SupportingTypes.IsRegistered(curRef, RegistrarMatches.TypeName))
                    {
                        continue;
                    }
                }

                var xenProp = new XenProperty
                {
                    Path = new[] { curRef.TargetName },
                    Value = curRef.GetTargetObject(),
                    CanRead = curRef.CanReadTarget,
                    CanWrite = curRef.CanWriteTarget
                };

                // is the current property an enumeration?
                if (curRef.IsTargetEnum && SupportingTypes.IsRegistered(typeof(Enum)))
                {
                    var value = curRef.As<Enum>();
                    if (value != null)
                    {
                        Descriptor.SetPossibleValues(curRef, xenProp, value);

                        if (xenProp.Value != null)
                        {
                            xenProp.Value = Enum.GetName(xenProp.Value.GetType(), value);
                        }
                    }
                }
                else
                {
                    Descriptor.SetPossibleValues(curRef, xenProp);

                    // non-primitive structures (not System.DateTime, but Xamarin.Forms.Point)
                    if (ReflectionMethods.IsNotPrimitiveValueType(xenProp.Value))
                    {
                        xenProp.XenType.Descriptor |= XenPropertyDescriptors.ValueType;

                        var vtProps = XenPropertyMethods
                            .GetPrimitiveValueTypeProperties(xenProp.Value)
                            .ToArray();

                        if (vtProps.Length != 0)
                        {
                            foreach (var vtProp in vtProps)
                            {
                                Descriptor.SetPossibleValues(vtProp);

                                if (vtProp.Value.GetType() != typeof(string))
                                {
                                    var tmp = Convert.ToString(vtProp.Value ?? string.Empty);
                                    vtProp.Value = tmp;
                                }
                            }

                            // don't return values to the toolbox that will never be used.
                            xenProp.Value = includeValues ? vtProps : null;
                        }
                    }

                    // enumerables, collections, list.
                    if (curRef.TargetType.IsKindOf(typeof(ICollection<>)))
                    {
                        xenProp.XenType.Descriptor |= XenPropertyDescriptors.Collection;
                    }

                    if (curRef.TargetType.IsKindOf(typeof(IList<>)))
                    {
                        xenProp.XenType.Descriptor |= XenPropertyDescriptors.List;
                    }

                    if (curRef.TargetType.IsKindOf(typeof(IEnumerable<>)))
                    {
                        xenProp.XenType.Descriptor |= XenPropertyDescriptors.Enumerable;
                        var collection = xenProp.Value as IEnumerable<object>;

                        if (collection != null)
                        {
                            var count = collection.Count();
                            xenProp.XenType.PossibleValues = new[] { count.ToString() };

                            if (includeValues == false)
                            {
                                // don't send the value back to the toolbox
                                xenProp.Value = null;
                            }
                        }
                    }

                    /*
                     * obj support?
                    if (!curRef.TargetType.GetTypeInfo().IsValueType && !curRef.TargetType.GetTypeInfo().IsPrimitive)
                    {
                        if (!curRef.TargetType.Namespace.StartsWith("System"))
                        {
                            var oProps = XenPropertyMethods
                            .GetObjectProperties(xenProp.Value)
                            .ToArray();

                            if (oProps.Length != 0)
                            {
                                foreach (var oProp in oProps)
                                {
                                    Descriptor.SetPossibleValues(oProp);

                                    if (oProp.Value != null && oProp.Value.GetType() != typeof(string))
                                    {
                                        var tmp = Convert.ToString(oProp.Value ?? string.Empty);
                                        oProp.Value = tmp;
                                    }
                                }

                                // don't return values to the toolbox that will never be used.
                                xenProp.Value = includeValues ? oProps : null;
                            }
                        }
                    }
                    */
                }

                result.Add(xenProp);
            }

            return result.ToArray();
        }


        /// <summary>
        /// The most common method to return an object's properties.
        /// Passing <paramref name="widgetId"/> alone will return all properties of the visual element.
        /// 
        /// A <paramref name="path"/> is an array of property names, starting after the widget.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="includeValues">Sets whether <see cref="XenProperty.Value"/> be set.</param>
        /// <param name="path"></param>
        protected XenProperty[] GetXenProperties(string widgetId, bool includeValues = false, string[] path = null)
        {
            if (string.IsNullOrWhiteSpace(widgetId)) return null;

            var pair = Surface[widgetId];
            if (pair.VisualElement == null) return null;

            // return all of the visual element's properties.
            var refProps = path == null
                ? pair.VisualElement.GetXenRefProperties()
                : pair.VisualElement.GetXenRefProperties(path);

            return refProps == null
                ? null
                : GetXenProperties(refProps, includeValues);
        }


        protected XenProperty[] GetXenProperties(object obj, bool includeValues = false)
        {
            if (obj == null) return null;
            var refProps = obj.GetXenRefProperties();
            return GetXenProperties(refProps, includeValues);
        }
    }
}