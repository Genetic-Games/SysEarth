using NUnit.Framework;
using SysEarth.Models;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class RootDirectoryTests
    {
        private Directory _root;
        private FileSystemState _state;

        [OneTimeSetUp]
        public void Setup()
        {
            _state = new FileSystemState();
            _root = _state.GetRootDirectory();
        }

        [Test]
        public void NewStateHasRootDirectory()
        {
            Assert.IsNotNull(_root);
        }

        public void NewStateRootDirectoryIsCurrentDirectory()
        {
            var current = _state.GetCurrentDirectory();
            Assert.AreEqual(_root, current);
        }

        [Test]
        public void RootDirectoryDoesNotHaveParentDirectory()
        {
            Assert.IsNull(_root.ParentDirectory);
        }

        [Test]
        public void RootDirectoryStartsWithNoSubDirectories()
        {
            Assert.IsNull(_root.SubDirectories);
        }

        [Test]
        public void RootDirectoryStartsWithNoFiles()
        {
            Assert.IsNull(_root.FilesInDirectory);
        }
    }
}
