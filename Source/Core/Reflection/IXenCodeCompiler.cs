namespace XenForms.Core.Reflection
{
    public interface IXenCodeCompiler
    {
        bool FromFiles(int names, string[] fileNames, out byte[] data);
        bool FromFile(string fileName, out byte[] data);
        bool FromSource(string code, out byte[] data);
    }
}