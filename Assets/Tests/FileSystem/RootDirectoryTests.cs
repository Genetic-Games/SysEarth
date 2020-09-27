using NUnit.Framework;
using SysEarth.States;
using System.Linq;

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
        public void NewStateRootDirectoryIsNotCurrentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var current = state.GetCurrentDirectory();

            Assert.AreNotEqual(root, current);
        }

        [Test]
        public void RootDirectoryDoesNotHaveParentDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            Assert.IsNull(root.ParentDirectory);
        }

        [Test]
        public void RootDirectoryStartsWithHomeSubDirectory()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            Assert.IsNotNull(root.SubDirectories);
            Assert.IsNotEmpty(root.SubDirectories);
            Assert.AreEqual(root.SubDirectories.Count, 1);
            Assert.AreEqual(root.SubDirectories.FirstOrDefault().Name, "home");
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
