using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class CurrentDirectoryTests
    {
        private DirectoryController _directoryController;

        [OneTimeSetUp]
        public void Setup()
        {
            _directoryController = new DirectoryController();
        }

        [Test]
        public void NewStateHasCurrentDirectory()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            Assert.IsNotNull(current);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoSubDirectories()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            Assert.IsNull(current.SubDirectories);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoFiles()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            Assert.IsNull(current.FilesInDirectory);
        }

        [Test]
        public void CurrentDirectoryStartsWithNoParentDirectory()
        {
            var state = new FileSystemState();
            var current = state.GetCurrentDirectory();
            Assert.IsNull(current.ParentDirectory);
        }

        [Test]
        public void CurrentDirectoryUnchangedWhenTargetDirectoryIsNull()
        {
            var state = new FileSystemState();
            var before = state.GetCurrentDirectory();
            var isSetDirectorySuccess = state.TrySetCurrentDirectory(null);
            var after = state.GetCurrentDirectory();

            Assert.IsFalse(isSetDirectorySuccess);
            Assert.AreEqual(before, after);
        }

        [Test]
        public void CanChangeCurrentDirectory()
        {
            var state = new FileSystemState();
            var before = state.GetCurrentDirectory();
            var target = new Directory();

            var isSetDirectorySuccess = state.TrySetCurrentDirectory(target);
            var after = state.GetCurrentDirectory();

            Assert.IsTrue(isSetDirectorySuccess);
            Assert.AreNotEqual(before, after);
            Assert.AreEqual(target, after);
        }
    }
}
