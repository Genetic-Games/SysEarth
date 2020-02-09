using SysEarth.Enums;
using SysEarth.Models;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Controllers
{
    public class FileController
    {
        public IList<File> GetFilesInDirectory(Directory target)
        {
            return target?.FilesInDirectory;
        }

        public bool TryGetFile(string fileName, FileExtension extension, Directory current, out File target)
        {
            target = null;

            // Cannot get file that does not exist
            var filesInDirectory = GetFilesInDirectory(current);
            if (filesInDirectory == null || !filesInDirectory.Any(x => x.Name == fileName && x.Extension == extension))
            {
                return false;
            }

            target = filesInDirectory.First(x => x.Name == fileName && x.Extension == extension);
            return true;
        }

        public bool TryAddFile(string fileName, FileExtension extension, Permission access, Directory current, out File target)
        {
            target = null;

            // Cannot create a file with the same name as an already existing file
            var filesInDirectory = GetFilesInDirectory(current);
            if (filesInDirectory != null && filesInDirectory.Any(x => x.Name == fileName))
            {
                return false;
            }

            // If file list does not exist yet, initialize it
            if (filesInDirectory == null)
            {
                filesInDirectory = new List<File>();
                current.FilesInDirectory = filesInDirectory;
            }

            // Add the target file
            target = new File
            {
                Access = access,
                Extension = extension,
                Name = fileName
            };
            filesInDirectory.Add(target);
            return true;
        }

        public bool TryDeleteFile(string fileName, FileExtension extension, Directory current)
        {
            // Cannot delete a file that does not exist
            var filesInDirectory = GetFilesInDirectory(current);
            if (filesInDirectory == null || !filesInDirectory.Any(x => x.Name == fileName && x.Extension == extension))
            {
                return false;
            }

            // Remove the target file
            var target = filesInDirectory.First(x => x.Name == fileName && x.Extension == extension);
            return filesInDirectory.Remove(target);
        }
    }
}
