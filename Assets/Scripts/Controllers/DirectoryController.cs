﻿using SysEarth.Models;
using System.Linq;

namespace SysEarth.Controllers
{
    public class DirectoryController
    {
        public bool TryGetDirectory(string directoryName, Directory current, out Directory target)
        {
            target = null;

            // Cannot get a subdirectory that does not exist
            if (current?.SubDirectories == null || !current.SubDirectories.Any(x => x.Name == directoryName))
            {
                return false;
            }

            // Get the target directory
            target = current.SubDirectories.First(x => x.Name == directoryName);
            return true;
        }

        public bool TryAddDirectory(string directoryName, Permission access, Directory current, out Directory target)
        {
            target = null;

            // Cannot create a subdirectory if the current directory cannot support it
            // Also cannot create a subdirectory with the same name as an already existing directory
            if (current?.SubDirectories == null || current.SubDirectories.Any(x => x.Name == directoryName))
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
            current.SubDirectories.Add(target);
            return true;
        }

        public bool TryDeleteDirectory(string directoryName, Directory current)
        {
            // Cannot delete a subdirectory that does not exist
            if (current?.SubDirectories == null || !current.SubDirectories.Any(x => x.Name == directoryName))
            {
                return false;
            }

            // Remove the target directory
            var target = current.SubDirectories.First(x => x.Name == directoryName);
            return current.SubDirectories.Remove(target);
        }
    }
}
