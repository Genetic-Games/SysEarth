﻿using SysEarth.Controllers;
using SysEarth.Enums;
using SysEarth.States;
using System.Collections.Generic;
using UnityEngine;

namespace SysEarth.Initializers
{
    public class FileSystemStateInitializer
    {
        public void InitializeFileSystemState(
            FileSystemState fileSystemState, 
            PermissionController permissionController, 
            DirectoryController directoryController)
        {
            // TODO - Make it so that all actions are permission based (add requires write access, so does delete, need read access to look at anything, execute to run something, etc)
            var root = fileSystemState.GetRootDirectory();

            var homeDirectoryPermission = permissionController.GetCustomPermission(canRead: true, canExecute: true);
            var isAddHomeDirectorySuccess = directoryController.TryAddDirectory("home", homeDirectoryPermission, root, out _);

            Debug.Assert(isAddHomeDirectorySuccess, $"Failed to add `home` directory under `{root.Name}` directory");
        }

        public void InitializeCommandsInFileSystemState(
            FileSystemState fileSystemState,
            PermissionController permissionController,
            DirectoryController directoryController,
            FileController fileController,
            IList<string> commandNames)
        {
            var root = fileSystemState.GetRootDirectory();

            var binDirectoryPermission = permissionController.GetCustomPermission(canRead: true, canExecute: true);
            var isAddBinDirectorySuccess = directoryController.TryAddDirectory("bin", binDirectoryPermission, root, out var binDirectory);

            Debug.Assert(isAddBinDirectorySuccess, $"Failed to add `bin` directory under `{root.Name}` directory");

            foreach (var commandName in commandNames)
            {
                var filePermission = permissionController.GetCustomPermission(canExecute: true);
                var isAddCommandFileSuccess = fileController.TryAddFile(commandName, FileExtension.exe, filePermission, binDirectory, out _);

                Debug.Assert(isAddCommandFileSuccess, $"Failed to add `{commandName}{FileExtension.exe.ToString()}` file under `{binDirectory.Name}` directory");
            }
        }
    }
}
