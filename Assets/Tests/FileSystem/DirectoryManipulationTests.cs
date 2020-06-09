using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;
using System.Linq;

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
        public void SubDirectoriesDoNotExistUntilCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            Assert.IsNotNull(root.SubDirectories);
            Assert.IsEmpty(root.SubDirectories);
        }

        [Test]
        public void SubDirectoriesExistWhenCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            // Need to store the boolean we want to test directly rather than the state 
            // This is because the underlying state will change but the reference pointer will not, leading to incorrect testing behavior
            var isSubDirectoriesNullBeforeAdd = root.SubDirectories == null;
            var isSubDirectoriesPopulatedBeforeAdd = root.SubDirectories.Any();
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var testDirectory);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.IsFalse(isSubDirectoriesNullBeforeAdd);
            Assert.IsFalse(isSubDirectoriesPopulatedBeforeAdd);
            Assert.IsNotNull(root.SubDirectories);
            Assert.That(root.SubDirectories.Contains(testDirectory));
        }

        [Test]
        public void CreatedDirectoryHasParentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var testDirectory);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.AreEqual(testDirectory.ParentDirectory, root);
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
        public void CannotGetParentDirectoryOfRootDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var isGetDirectorySuccess = _directoryController.TryGetDirectory("..", root, out var target);

            Assert.IsFalse(isGetDirectorySuccess);
            Assert.IsNull(target);
        }

        [Test]
        public void CanGetParentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var isAddDirectorySuccess = _directoryController.TryAddDirectory("Test", new Permission(), root, out var testDirectory);
            var isGetDirectorySuccess = _directoryController.TryGetDirectory("..", testDirectory, out var target);

            Assert.IsTrue(isAddDirectorySuccess);
            Assert.AreEqual(testDirectory.ParentDirectory, root);
            Assert.IsTrue(isGetDirectorySuccess);
            Assert.AreEqual(target, root);
        }

        [Test]
        public void CanGetCurrentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var isGetDirectorySuccess = _directoryController.TryGetDirectory(".", root, out var target);

            Assert.IsTrue(isGetDirectorySuccess);
            Assert.AreEqual(target, root);
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
