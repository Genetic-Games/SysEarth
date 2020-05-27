using SysEarth.Enums;
using SysEarth.Models;
using System.Linq;

namespace SysEarth.Controllers
{
    public class FileController
    {
        public bool TryGetFile(string fileName, FileExtension extension, Directory current, out File target)
        {
            target = null;

            // Cannot get file that does not exist
            if (current?.FilesInDirectory == null || !current.FilesInDirectory.Any(x => x.Name == fileName && x.Extension == extension))
            {
                return false;
            }

            target = current.FilesInDirectory.First(x => x.Name == fileName && x.Extension == extension);
            return true;
        }

        public bool TryAddFile(string fileName, FileExtension extension, Permission access, Directory current, out File target)
        {
            target = null;

            // Cannot create a file in a directory if the current directory cannot support it
            // Also cannot create a file with the same name as an already existing file
            if (current?.FilesInDirectory != null && current.FilesInDirectory.Any(x => x.Name == fileName))
            {
                return false;
            }

            // Add the target file
            target = new File
            {
                Access = access,
                Extension = extension,
                Name = fileName
            };

            current.FilesInDirectory.Add(target);
            return true;
        }

        public bool TryDeleteFile(string fileName, FileExtension extension, Directory current)
        {
            // Cannot delete a file that does not exist
            if (current?.FilesInDirectory == null || !current.FilesInDirectory.Any(x => x.Name == fileName && x.Extension == extension))
            {
                return false;
            }

            // Remove the target file
            var target = current.FilesInDirectory.First(x => x.Name == fileName && x.Extension == extension);
            return current.FilesInDirectory.Remove(target);
        }
    }
}
