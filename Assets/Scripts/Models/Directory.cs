using System.Collections.Generic;

namespace SysEarth.Models
{
    public class Directory
    {
        public string Name { get; set; }

        public Directory ParentDirectory { get; set; }

        public IList<File> FilesInDirectory { get; set; } = new List<File>();

        public IList<Directory> SubDirectories { get; set; } = new List<Directory>();

        public Permission Access { get; set; }
    }
}
