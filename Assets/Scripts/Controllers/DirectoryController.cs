﻿using SysEarth.Models;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Controllers
{
    public class DirectoryController
    {
        private const string _currentDirectorySymbol = ".";
        private const string _parentDirectorySymbol = "..";
        private const string _rootDirectorySymbol = "/";
        private const string _homeDirectorySymbol = "~";

        private const string _homeDirectoryIndicator = "home";
        private const string _directoryIndicator = "/";

        public bool TryGetDirectory(string directoryName, Directory current, out Directory target)
        {
            target = null;

            // Special case where current directory is requested
            if (directoryName == _currentDirectorySymbol)
            {
                target = current;
                return true;
            }

            // Special case where parent directory is requested
            if (directoryName == _parentDirectorySymbol)
            {
                if (current?.ParentDirectory != null)
                {
                    target = current.ParentDirectory;
                    return true;
                }

                return false;
            }

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

        public string GetDirectoryPath(Directory target)
        {
            if (target == null)
            {
                return null;
            }

            // Handle the special case where no parent directory exists
            if (target.ParentDirectory == null)
            {
                return target.Name;
            }

            // Loop through every directory from the target to root (non-inclusive) and add each one
            var directoryNames = new List<string>();
            while (target.ParentDirectory != null)
            {
                directoryNames.Add(target.Name);
                target = target.ParentDirectory;
            }

            // Add the top level directory name (the one without a parent), but if it is root, treat it as empty since path separators will handle it
            if (target.Name == _rootDirectorySymbol)
            {
                directoryNames.Add(string.Empty);
            }
            else
            {
                directoryNames.Add(target.Name);
            }

            // Spin the list around so it is in the larger to smaller direction (root to target, left to right)
            directoryNames.Reverse();
            var fullyQualifiedPathString = string.Join(_directoryIndicator, directoryNames);

            // Finally, check if a replacement for the home directory can be made to make the path shorter and easier to use
            var shortenedPathString = fullyQualifiedPathString.Replace($"{_rootDirectorySymbol}{_homeDirectoryIndicator}", _homeDirectorySymbol);
            return shortenedPathString;
        }
    }
}
