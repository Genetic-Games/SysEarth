using SysEarth.Controllers;
using SysEarth.Models;

namespace SysEarth.States
{
    public class FileSystemState
    {
        private Directory _currentDirectory;
        private readonly Directory _rootDirectory;
        private readonly Directory _homeDirectory;

        public FileSystemState(Permission rootAccess = null, Permission homeAccess = null)
        {
            if (rootAccess == null)
            {
                var permissionController = new PermissionController();
                rootAccess = permissionController.GetCustomPermission(canRead: true, canExecute: true);
            }

            _rootDirectory = new Directory
            {
                Name = "/",
                Access = rootAccess
            };

            if (homeAccess == null)
            {
                var permissionController = new PermissionController();
                homeAccess = permissionController.GetCustomPermission(canRead: true, canExecute: true);
            }

            _homeDirectory = new Directory
            {
                Name = "home",
                Access = homeAccess,
                ParentDirectory = _rootDirectory
            };

            _rootDirectory.SubDirectories.Add(_homeDirectory);
            _currentDirectory = _homeDirectory;
        }

        public Directory GetRootDirectory()
        {
            return _rootDirectory;
        }

        public Directory GetHomeDirectory()
        {
            return _homeDirectory;
        }

        public Directory GetCurrentDirectory()
        {
            return _currentDirectory;
        }

        public bool TrySetCurrentDirectory(Directory target)
        {
            if (target == null)
            {
                return false;
            }

            _currentDirectory = target;
            return true;
        }
    }
}
