using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class PathTests
    {
        private DirectoryController _directoryController;

        [OneTimeSetUp]
        public void Setup()
        {
            _directoryController = new DirectoryController();
        }

        [Test]
        public void CannotGetPathForNullDirectory()
        {
            var path = _directoryController.GetDirectoryPath(null);

            Assert.IsNull(path);
        }

        [Test]
        public void RootDirectoryPathIsDirectoryName()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var path = _directoryController.GetDirectoryPath(root);
            var expectedPath = "/";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void DirectoryWithNoParentPathIsDirectoryName()
        {
            var directory = new Directory
            {
                ParentDirectory = null,
                Name = "Test"
            };

            var path = _directoryController.GetDirectoryPath(directory);
            var expectedPath = "Test";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void DirectoryPathIncludesParentDirectory()
        {
            var parentDirectory = new Directory
            {
                Name = "Parent",
                ParentDirectory = null
            };

            var childDirectory = new Directory
            {
                Name = "Child",
                ParentDirectory = parentDirectory
            };

            var path = _directoryController.GetDirectoryPath(childDirectory);
            var expectedPath = "Parent/Child";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void RootDirectoryPathIncludesRoot()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var parentDirectory = new Directory
            {
                Name = "Parent",
                ParentDirectory = root
            };

            var childDirectory = new Directory
            {
                Name = "Child",
                ParentDirectory = parentDirectory
            };

            var path = _directoryController.GetDirectoryPath(childDirectory);
            var expectedPath = "/Parent/Child";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void DirectoryPathIncludesRootDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var childDirectory = new Directory
            {
                Name = "Child",
                ParentDirectory = root
            };

            var path = _directoryController.GetDirectoryPath(childDirectory);
            var expectedPath = "/Child";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void HomeDirectoryPathIsShortened()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var homeDirectory = new Directory
            {
                Name = "home",
                ParentDirectory = root
            };

            var path = _directoryController.GetDirectoryPath(homeDirectory);
            var expectedPath = "~";

            Assert.AreEqual(path, expectedPath);
        }

        [Test]
        public void HomeDirectoryPathWithChildIsShortened()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var homeDirectory = new Directory
            {
                Name = "home",
                ParentDirectory = root
            };

            var childDirectory = new Directory
            {
                Name = "Child",
                ParentDirectory = homeDirectory
            };

            var path = _directoryController.GetDirectoryPath(childDirectory);
            var expectedPath = "~/Child";

            Assert.AreEqual(path, expectedPath);
        }
    }
}
