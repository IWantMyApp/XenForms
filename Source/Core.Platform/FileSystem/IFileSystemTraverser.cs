using System;
using XenForms.Core.FileSystem;

namespace XenForms.Core.Platform.FileSystem
{
    public interface IFileSystemTraverser
    {
        void RecurseFolder(string path, bool includeTopFolder, Action<FolderDesc> topFolderFound, Action<FolderDesc> subFolderFound = null);
        FileDesc[] EnumerateTopFiles(string path, params string[] extensions);
        void EnumerateTopFolders(string path, Action<FolderDesc> found, bool includeRoot = false);
        bool FilesExist(string path, params string[] extensions);
    }
}