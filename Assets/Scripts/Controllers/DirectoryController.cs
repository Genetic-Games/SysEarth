using SysEarth.Models;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Controllers
{
    public class DirectoryController
    {
        public Directory GetParentDirectory(Directory target)
        {
            return target?.ParentDirectory;
        }

        public IList<Directory> GetSubDirectories(Directory target)
        {
            return target?.SubDirectories;
        }

        public bool TryGetDirectory(string directoryName, Directory current, out Directory target)
        {
            target = null;

            // Cannot get a subdirectory that does not exist
            var subDirectories = GetSubDirectories(current);
            if (subDirectories == null || !subDirectories.Any(x => x.Name == directoryName))
            {
                return false;
            }

            // Get the target directory
            target = subDirectories.First(x => x.Name == directoryName);
            return true;
        }

        public bool TryAddDirectory(string directoryName, Permission access, Directory current, out Directory target)
        {
            target = null;

            // Cannot create a subdirectory with the same name as an already existing directory
            var subDirectories = GetSubDirectories(current);
            if (subDirectories != null && subDirectories.Any(x => x.Name == directoryName))
            {
                return false;
            }

            // Add the target directory
            target = new Directory
            {
                Access = access,
                Name = directoryName,
                ParentDirectory = current
            };
            subDirectories.Add(target);
            return true;
        }

        public bool TryDeleteDirectory(string directoryName, Directory current)
        {
            // Cannot delete a subdirectory that does not exist
            var subDirectories = GetSubDirectories(current);
            if (subDirectories == null || !subDirectories.Any(x => x.Name == directoryName))
            {
                return false;
            }

            // Remove the target directory
            var target = subDirectories.First(x => x.Name == directoryName);
            return subDirectories.Remove(target);
        }
    }
}
