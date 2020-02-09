using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class CurrentDirectoryTests
    {
        private FileSystemState _state;

        [OneTimeSetUp]
        public void Setup()
        {
            _state = new FileSystemState();
        }

        [Test]
        public void NewStateHasCurrentDirectory()
        {
            var current = _state.GetCurrentDirectory();
            Assert.IsNotNull(current);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoSubDirectories()
        {
            var current = _state.GetCurrentDirectory();
            Assert.IsNull(current.SubDirectories);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoFiles()
        {
            var current = _state.GetCurrentDirectory();
            Assert.IsNull(current.FilesInDirectory);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoParentDirectory()
        {
            var current = _state.GetCurrentDirectory();
            Assert.IsNull(current.ParentDirectory);
        }
    }
}
