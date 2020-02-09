using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class PermissionsTests
    {
        private PermissionController _permissionController;

        [OneTimeSetUp]
        public void Setup()
        {
            _permissionController = new PermissionController();
        }

        [Test]
        public void GetNullFilePermissionGivesNullPermission()
        {
            File nullFile = null;
            var access = _permissionController.GetPermissions(nullFile);
            Assert.IsNull(access);
        }

        [Test]
        public void GetNullDirectoryPermissionGivesNullPermission()
        {
            Directory nullDirectory = null;
            var access = _permissionController.GetPermissions(nullDirectory);
            Assert.IsNull(access);
        }

        [Test]
        public void RootDirectoryHasReadAccess()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsTrue(rootAccess.Read);
        }

        [Test]
        public void RootDirectoryDoesNotHaveWriteAccess()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsFalse(rootAccess.Write);
        }

        [Test]
        public void RootDirectoryHasExecuteAccess()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var rootAccess = _permissionController.GetPermissions(root);
            Assert.IsTrue(rootAccess.Execute);
        }

        [Test]
        public void CurrentDirectoryHasReadAccess()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsTrue(currentAccess.Read);
        }

        [Test]
        public void CurrentDirectoryDoesNotHaveWriteAccess()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsFalse(currentAccess.Write);
        }

        [Test]
        public void CurrentDirectoryHasExecuteAccess()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            var currentAccess = _permissionController.GetPermissions(current);
            Assert.IsTrue(currentAccess.Execute);
        }

        [Test]
        public void GetCustomPermissionGivesDefaultsToFalseEverywhere()
        {
            var access = _permissionController.GetCustomPermission();
            Assert.IsFalse(access.Read);
            Assert.IsFalse(access.Write);
            Assert.IsFalse(access.Execute);
        }

        [Test]
        public void GetCustomPermissionGivesMatchingPermission()
        {
            var canRead = true;
            var canWrite = true;
            var canExecute = true;

            var access = _permissionController.GetCustomPermission(canRead, canWrite, canExecute);

            Assert.AreEqual(access.Read, canRead);
            Assert.AreEqual(access.Write, canWrite);
            Assert.AreEqual(access.Execute, canExecute);
        }

        [Test]
        public void CannotSetFilePermissionsForFileThatDoesNotExist()
        {
            File nullFile = null;
            var isSetFilePermission = _permissionController.TrySetPermissions(nullFile, new Permission());

            Assert.IsFalse(isSetFilePermission);
        }

        [Test]
        public void CannotSetDirectoryPermissionsForDirectoryThatDoesNotExist()
        {
            Directory nullDirectory = null;
            var isSetDirectoryPermission = _permissionController.TrySetPermissions(nullDirectory, new Permission());

            Assert.IsFalse(isSetDirectoryPermission);
        }

        [Test]
        public void CanSetFilePermissionsForFileThatExists()
        {
            var file = new File();
            var before = _permissionController.GetPermissions(file);

            var isSetFilePermission = _permissionController.TrySetPermissions(file, new Permission());
            var after = _permissionController.GetPermissions(file);

            Assert.IsNull(before);
            Assert.IsTrue(isSetFilePermission);
            Assert.IsNotNull(after);
        }

        [Test]
        public void CanSetDirectoryPermissionsForDirectoryThatExists()
        {
            var directory = new Directory();
            var before = _permissionController.GetPermissions(directory);

            var isSetDirectoryPermission = _permissionController.TrySetPermissions(directory, new Permission());
            var after = _permissionController.GetPermissions(directory);

            Assert.IsNull(before);
            Assert.IsTrue(isSetDirectoryPermission);
            Assert.IsNotNull(after);
        }
    }
}
