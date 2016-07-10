namespace XenForms.Core.Reflection
{
    public interface IShallowClone<out T>
    {
        T ShallowClone();
    }
}