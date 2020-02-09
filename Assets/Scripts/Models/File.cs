using SysEarth.Enums;

namespace SysEarth.Models
{
    public class File
    {
        public string Name { get; set; }

        public FileExtension Extension { get; set; }

        public Permission Access { get; set; }
    }
}
