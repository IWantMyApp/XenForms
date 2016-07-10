
namespace XenForms.Core.Toolbox
{
    public interface IPropertyEditor<T>
    {
        PropertyEditorModel<T> Model { get; }
    }
}