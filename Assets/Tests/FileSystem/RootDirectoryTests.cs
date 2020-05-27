using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class RootDirectoryTests
    {
        [Test]
        public void NewStateHasRootDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            Assert.IsNotNull(root);
        }

        [Test]
        public void NewStateHasRootDirectoryNamedSlash()
        {
            var state = new FileSystemState();
            var expected = "/";
            var root = state.GetRootDirectory();
            Assert.IsNotNull(root);
            Assert.AreEqual(root.Name, expected);
        }

        [Test]
        public void NewStateRootDirectoryIsCurrentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var current = state.GetCurrentDirectory();
            Assert.AreEqual(root, current);
        }

        [Test]
        public void RootDirectoryDoesNotHaveParentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            Assert.IsNull(root.ParentDirectory);
        }

        [Test]
        public void RootDirectoryStartsWithNoSubDirectories()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            Assert.IsNotNull(root.SubDirectories);
            Assert.IsEmpty(root.SubDirectories);
        }

        [Test]
        public void RootDirectoryStartsWithNoFiles()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            Assert.IsNotNull(root.FilesInDirectory);
            Assert.IsEmpty(root.FilesInDirectory);
        }
    }
}
