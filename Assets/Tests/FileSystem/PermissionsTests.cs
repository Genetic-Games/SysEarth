using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class PermissionsTests
    {
        private FileSystemState _state;
        private PermissionController _permissionController;

        [OneTimeSetUp]
        public void Setup()
        {
            _state = new FileSystemState();
            _permissionController = new PermissionController();
        }

        [Test]
        public void RootDirectoryHasReadAccess()
        {
            var root = _state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsTrue(rootAccess.Read);
        }

        [Test]
        public void RootDirectoryDoesNotHaveWriteAccess()
        {
            var root = _state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsFalse(rootAccess.Write);
        }

        [Test]
        public void RootDirectoryHasExecuteAccess()
        {
            var root = _state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsTrue(rootAccess.Execute);
        }

        [Test]
        public void CurrentDirectoryHasReadAccess()
        {
            var current = _state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsTrue(currentAccess.Read);
        }

        [Test]
        public void CurrentDirectoryDoesNotHaveWriteAccess()
        {
            var current = _state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsFalse(currentAccess.Write);
        }

        [Test]
        public void CurrentDirectoryHasExecuteAccess()
        {
            var current = _state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsTrue(currentAccess.Execute);
        }

        // TODO - Add more tests here for permissions
    }
}
