using SysEarth.Controllers;
using SysEarth.Models;

namespace SysEarth.States
{
    public class FileSystemState
    {
        private Directory _currentDirectory;
        private Directory _rootDirectory;

        public FileSystemState(Permission rootAccess = null)
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
            _currentDirectory = _rootDirectory;
        }

        public Directory GetRootDirectory()
        {
            return _rootDirectory;
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
