using System;

namespace XenForms.Core.Reflection
{
    public struct TypeAttributeAssociation<T> where T : Attribute
    {
        public T[] Attributes { get; set; }
        public Type DecoratedType { get; set; }
    }


    public interface IFindCustomAttributes<T> where T : Attribute
    {
        TypeAttributeAssociation<T>[] FindAll();
    }
}