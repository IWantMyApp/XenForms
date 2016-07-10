using System;
using System.IO;
using System.Reflection;
using XenForms.Core.FileSystem;

namespace XenForms.Core.Platform.FileSystem
{
    public class FileSystem : IFileSystem
    {
        public bool FileExist(string path)
        {
            return File.Exists(path);
        }


        public bool ReadAllText(string path, out string output)
        {
            if (string.IsNullOrWhiteSpace(path) || !FileExist(path))
            {
                output = null;
                return false;
            }

            output = File.ReadAllText(path);
            return true;
        }


        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }


        public string Combine(params string[] path)
        {
            return Path.Combine(path);
        }


        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}