namespace XenForms.Core.FileSystem
{
    public interface IFileSystem
    {
        bool FileExist(string path);
        bool ReadAllText(string path, out string output);
        void WriteAllText(string path, string contents);
        string Combine(params string[] path);
    }
}