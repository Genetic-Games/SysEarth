using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class Directory
    {
        public string Name { get; set; }

        public IList<File> FilesInDirectory { get; set; }

        public IList<Directory> SubDirectories { get; set; }

        public Permission Access { get; set; }
    }
}
