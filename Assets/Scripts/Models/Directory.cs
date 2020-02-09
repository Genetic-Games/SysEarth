using System.Collections.Generic;

namespace SysEarth.Models
{
    public class Directory
    {
        public string Name { get; set; }

        public Directory ParentDirectory { get; set; }

        public IList<File> FilesInDirectory { get; set; }

        public IList<Directory> SubDirectories { get; set; }

        public Permission Access { get; set; }
    }
}
