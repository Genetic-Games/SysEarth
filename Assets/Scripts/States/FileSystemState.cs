using Assets.Scripts.Models;

namespace Assets.Scripts.States
{
    public class FileSystemState
    {
        private Directory _currentDirectory;
        private Directory _rootDirectory;

        public FileSystemState()
        {
            _rootDirectory = new Directory();
            _currentDirectory = _rootDirectory;
        }

        public void ChangeCurrentDirectory(Directory target)
        {
            _currentDirectory = target;
        }

        public Directory GetCurrentDirectory(Directory target)
        {
            return _currentDirectory;
        }

        public Permission GetFilePermissions(File target)
        {
            return target.Access;
        }

        public void SetFilePermissions(File targetFile, Permission targetAccess)
        {
            targetFile.Access = targetAccess;
        }
    }
}
