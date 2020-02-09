using NUnit.Framework;
using SysEarth.Models;
using SysEarth.States;

namespace SysEarth.Tests.FileSystem
{
    public class DirectoryTraversalTests
    {
        private FileSystemState _state;

        [OneTimeSetUp]
        public void Setup()
        {
            _state = new FileSystemState();
        }

        [Test]
        public void CurrentDirectoryUnchangedWhenTargetDirectoryIsNull()
        {
            var before = _state.GetCurrentDirectory();
            var isSetDirectorySuccess = _state.TrySetCurrentDirectory(null);
            var after = _state.GetCurrentDirectory();

            Assert.IsFalse(isSetDirectorySuccess);
            Assert.AreEqual(before, after);
        }

        [Test]
        public void CanChangeCurrentDirectory()
        {
            var before = _state.GetCurrentDirectory();
            var target = new Directory
            {
                Name = "ChangedDirectory"
            };

            var isSetDirectorySuccess = _state.TrySetCurrentDirectory(target);
            var after = _state.GetCurrentDirectory();

            Assert.IsTrue(isSetDirectorySuccess);
            Assert.AreNotEqual(before, after);
            Assert.AreEqual(target, after);
        }

        // TODO - Add more tests here surrounding directory traversal
    }
}
