namespace XenForms.Core.FileSystem
{
    public class FolderDesc
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public bool HasFolders { get; set; }
        public bool HasFiles { get; set; }
        public bool HasEntries => HasFolders || HasFiles;
    }
}