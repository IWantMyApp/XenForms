using XenForms.Core.FileSystem;

namespace XenForms.Core.Reflection
{
    public class XenCodeEditor
    {
        private readonly IFileSystem _fs;
        private readonly string _sourceFile;


        public XenCodeEditor(IFileSystem fs, string sourceFile)
        {
            _fs = fs;
            _sourceFile = sourceFile;
        }


        public bool PrependEventHandler(string methodName, string arguments)
        {
            return false;
        }
    }
}