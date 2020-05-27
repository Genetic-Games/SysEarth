using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Enums;
using SysEarth.Models;
using SysEarth.States;
using System.Linq;

namespace SysEarth.Tests.FileSystem
{
    public class FileManipulationTests
    {
        private FileController _fileController;

        [OneTimeSetUp]
        public void Setup()
        {
            _fileController = new FileController();
        }

        [Test]
        public void FilesInDirectoryDoNotExistUntilCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            Assert.IsNotNull(root.FilesInDirectory);
            Assert.IsEmpty(root.FilesInDirectory);
        }

        [Test]
        public void FilesInDirectoryExistWhenCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            // Need to store the boolean we want to test directly rather than the state 
            // This is because the underlying state will change but the reference pointer will not, leading to incorrect testing behavior
            var isFilesInDirectoryNullBeforeAdd = root.FilesInDirectory == null;
            var isFilesInDirectoryPopulatedBeforeAdd = root.FilesInDirectory.Any();
            var isAddFileSuccess = _fileController.TryAddFile("Test", FileExtension.None, new Permission(), root, out var testFile);

            Assert.IsTrue(isAddFileSuccess);
            Assert.IsFalse(isFilesInDirectoryNullBeforeAdd);
            Assert.IsFalse(isFilesInDirectoryPopulatedBeforeAdd);
            Assert.IsNotNull(root.FilesInDirectory);
            Assert.That(root.FilesInDirectory.Contains(testFile));
        }

        [Test]
        public void CannotGetFileThatDoesNotExist()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var isGetFileSuccess = _fileController.TryGetFile("Test", FileExtension.None, root, out var target);

            Assert.IsFalse(isGetFileSuccess);
            Assert.IsNull(target);
        }

        [Test]
        public void CanGetFileThatExists()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddFileSuccess = _fileController.TryAddFile("Test", FileExtension.None, new Permission(), root, out var addedFile);

            var isGetFileSuccess = _fileController.TryGetFile("Test", FileExtension.None, root, out var retrievedFile);

            Assert.IsTrue(isAddFileSuccess);
            Assert.IsTrue(isGetFileSuccess);
            Assert.AreEqual(addedFile, retrievedFile);
        }

        [Test]
        public void CanDeleteFileThatExists()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isAddFileSuccess = _fileController.TryAddFile("Test", FileExtension.None, new Permission(), root, out var addedFile);

            var isDeleteFileSuccess = _fileController.TryDeleteFile("Test", FileExtension.None, root);

            Assert.IsTrue(isAddFileSuccess);
            Assert.IsTrue(isDeleteFileSuccess);
        }

        [Test]
        public void CannotDeleteFileThatDoesNotExist()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();
            var isDeleteFileSuccess = _fileController.TryDeleteFile("Test", FileExtension.None, root);

            Assert.IsFalse(isDeleteFileSuccess);
        }
    }
}
