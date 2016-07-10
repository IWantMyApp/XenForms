using Xamarin.Forms;

namespace XenForms.Designer.Tests.Reactions.SetWidgetProperties
{
    // must be a struct
    public struct FakeStruct
    {
        public InnerStruct Inner { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public struct InnerStruct
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class FakeClass
    {
        public string Name { get; set; }
    }

    public class ViewHasSubProps : ContentView
    {
        public FakeStruct FakeVal { get; set; }
        public FakeClass FakeRef { get; set; }
    }
}