using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;
using System.Collections;

namespace SysEarth.Tests.FileSystem
{
    class DirectoryManipulationTests
    {
        private DirectoryController _directoryController;

        [OneTimeSetUp]
        public void Setup()
        {
            _directoryController = new DirectoryController();
        }

        [Test]
        public void NullDirectoryHasNullSubDirectories()
        {
            var subDirectories = _directoryController.GetSubDirectories(null);
            Assert.IsNull(subDirectories);
        }

        [Test]
        public void SubDirectoriesDoNotExistUntilCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var subDirectories = _directoryController.GetSubDirectories(root);

            Assert.IsNull(subDirectories);
        }

        [Test]
        public void SubDirectoriesExistWhenCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var before = _directoryController.GetSubDirectories(root);
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var testDirectory);

            var after = _directoryController.GetSubDirectories(root);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.IsNull(before);
            Assert.IsNotNull(after);
            Assert.Contains(testDirectory, (ICollection) after);
        }

        [Test]
        public void NullDirectoryHasNullParentDirectory()
        {
            var parent = _directoryController.GetParentDirectory(null);
            Assert.IsNull(parent);
        }

        [Test]
        public void CreatedDirectoryHasParentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var testDirectory);
            var parent = _directoryController.GetParentDirectory(testDirectory);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.AreEqual(parent, root);
        }

        [Test]
        public void CannotGetDirectoryThatDoesNotExist()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var isGetDirectorySuccess = _directoryController.TryGetDirectory("Test", root, out var target);

            Assert.IsFalse(isGetDirectorySuccess);
            Assert.IsNull(target);
        }

        [Test]
        public void CanGetDirectoryThatExists()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var addedDirectory);

            var isGetDirectorySuccess = _directoryController.TryGetDirectory("Test", root, out var retrievedDirectory);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.IsTrue(isGetDirectorySuccess);
            Assert.AreEqual(addedDirectory, retrievedDirectory);
        }

        [Test]
        public void CanDeleteDirectoryThatExists()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var addedDirectory);

            var isDeleteDirectorySuccess = _directoryController.TryDeleteDirectory("Test", root);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.IsTrue(isDeleteDirectorySuccess);
        }

        [Test]
        public void CannotDeleteDirectoryThatDoesNotExist()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isDeleteDirectorySuccess = _directoryController.TryDeleteDirectory("Test", root);

            Assert.IsFalse(isDeleteDirectorySuccess);
        }
    }
}
