using NUnit.Framework;
using SysEarth.Controllers;
using SysEarth.Enums;
using SysEarth.Models;
using SysEarth.States;
using System.Collections;

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
        public void NullDirectoryHasNullFilesInDirectory()
        {
            var filesInDirectory = _fileController.GetFilesInDirectory(null);
            Assert.IsNull(filesInDirectory);
        }

        [Test]
        public void FilesInDirectoryDoNotExistUntilCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var filesInDirectory = _fileController.GetFilesInDirectory(root);

            Assert.IsNull(filesInDirectory);
        }

        [Test]
        public void FilesInDirectoryExistWhenCreated()
        {
            var state = new FileSystemState();
            var root = state.GetRootDirectory();

            var before = _fileController.GetFilesInDirectory(root);
            var isAddFileSuccess = _fileController.TryAddFile("Test", FileExtension.None, new Permission(), root, out var testFile);

            var after = _fileController.GetFilesInDirectory(root);

            Assert.IsTrue(isAddFileSuccess);
            Assert.IsNull(before);
            Assert.IsNotNull(after);
            Assert.That(after.Contains(testFile));
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
