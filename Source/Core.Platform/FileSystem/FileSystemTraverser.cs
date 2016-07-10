using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XenForms.Core.FileSystem;

namespace XenForms.Core.Platform.FileSystem
{
    public class FileSystemTraverser : IFileSystemTraverser
    {
        private const string AnyPattern = "*";


        public void RecurseFolder(string path, bool includeTopFolder, Action<FolderDesc> topFolderFound, Action<FolderDesc> subFolderFound = null)
        {
            if (topFolderFound == null)
            {
                throw new InvalidOperationException($"{nameof(topFolderFound)} can't be null.");
            }

            if (!IsSupportedFolder(path))
            {
                return;
            }

            EnumerateTopFolders(path, top =>
            {
                topFolderFound(top);

                if (top.HasEntries && subFolderFound != null)
                {
                    EnumerateTopFolders(top.FullPath, subFolderFound);
                }
            }, includeTopFolder);
        }


        public FileDesc[] EnumerateTopFiles(string path, params string[] extensions)
        {
            var results = new List<FileDesc>();
            string[] files;

            if (extensions.Length == 0 || extensions[0] == AnyPattern)
            {
                files = Directory
                    .EnumerateFiles(path, AnyPattern, SearchOption.TopDirectoryOnly)
                    .OrderBy(f => f)
                    .ToArray();
            }
            else
            {
                files = Directory
                    .EnumerateFiles(path, AnyPattern, SearchOption.TopDirectoryOnly)
                    .Where(f => extensions.Contains(Path.GetExtension(f)))
                    .OrderBy(f => f)
                    .ToArray();
            }

            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                if (!fi.Exists) continue;

                var item = new FileDesc
                {
                    FullPath = fi.FullName,
                    Name = fi.Name,
                    ParentDirectory = fi.Directory?.FullName
                };

                results.Add(item);
            }

            return results.ToArray();
        }


        public void EnumerateTopFolders(string path, Action<FolderDesc> found, bool includeRoot = false)
        {
            try
            {
                var folders = includeRoot ? new[] {path} : GetSubFolders(path).ToArray();

                foreach (var folder in folders)
                {
                    if (!IsSupportedFolder(folder))
                    {
                        continue;
                    }

                    var di = new DirectoryInfo(folder);
                    var data = new FolderDesc
                    {
                        HasFiles = EnumerateTopFiles(folder, AnyPattern).Any(),
                        HasFolders = GetSubFolders(folder).Any(),
                        FullPath = di.FullName,
                        Name = di.Name
                    };

                    found(data);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // ignored
            }
            catch (DirectoryNotFoundException)
            {
                // ignored
            }
        }


        public bool FilesExist(string path, params string[] extensions)
        {
            return Directory
                .EnumerateFiles(path, AnyPattern, SearchOption.TopDirectoryOnly)
                .Where(f => extensions.Contains(Path.GetExtension(f)))
                .Any(IsSupportedFolder);
        }


        private bool IsSupportedFolder(string path)
        {
            var di = new DirectoryInfo(path);
            var att = di.Attributes;
            var root = Directory.GetDirectoryRoot(path);

            if (path == root) return true;
            if (!di.Exists) return false;
            if (att.HasFlag(FileAttributes.Hidden)) return false;
            if (att.HasFlag(FileAttributes.Temporary)) return false;
            if (att.HasFlag(FileAttributes.System)) return false;

            return true;
        }


        private IEnumerable<string> GetSubFolders(string path)
        {
            return Directory
                .EnumerateDirectories(path, AnyPattern, SearchOption.TopDirectoryOnly)
                .OrderBy(d => d);
        }
    }
}
