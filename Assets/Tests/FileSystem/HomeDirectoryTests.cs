using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class HomeDirectoryTests
    {
        [Test]
        public void NewStateHasHomeDirectory()
        {
            var state = new FileSystemState();
            var home = state.GetHomeDirectory();

            Assert.IsNotNull(home);
        }

        [Test]
        public void NewStateHasHomeDirectoryNamedHome()
        {
            var state = new FileSystemState();
            var expected = "home";
            var home = state.GetHomeDirectory();

            Assert.IsNotNull(home);
            Assert.AreEqual(home.Name, expected);
        }

        [Test]
        public void NewStateHomeDirectoryIsCurrentDirectory()
        {
            var state = new FileSystemState();
            var home = state.GetHomeDirectory();
            var current = state.GetCurrentDirectory();

            Assert.AreEqual(home, current);
        }

        [Test]
        public void HomeDirectoryHasRootParentDirectory()
        {
            var state = new FileSystemState();
            var home = state.GetHomeDirectory();
            var root = state.GetRootDirectory();

            Assert.IsNotNull(home.ParentDirectory);
            Assert.AreEqual(home.ParentDirectory, root);
        }

        [Test]
        public void HomeDirectoryStartsWithNoSubDirectories()
        {
            var state = new FileSystemState();
            var home = state.GetHomeDirectory();

            Assert.IsNotNull(home.SubDirectories);
            Assert.IsEmpty(home.SubDirectories);
        }

        [Test]
        public void HomeDirectoryStartsWithNoFiles()
        {
            var state = new FileSystemState();
            var home = state.GetHomeDirectory();

            Assert.IsNotNull(home.FilesInDirectory);
            Assert.IsEmpty(home.FilesInDirectory);
        }
    }
}
