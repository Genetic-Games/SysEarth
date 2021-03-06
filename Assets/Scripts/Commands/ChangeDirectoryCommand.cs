﻿using SysEarth.Controllers;
using SysEarth.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Commands
{
    public class ChangeDirectoryCommand : ICommand
    {
        private const string _currentDirectorySymbol = ".";
        private const string _parentDirectorySymbol = "..";
        private const string _homeDirectorySymbol = "~";
        private const string _homeDirectoryName = "home";

        private readonly IList<char> _pathDelimiters = new List<char> { '\\', '/' };

        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;
        private readonly DirectoryController _directoryController;

        // Interface Fields
        private readonly string _commandName = "cd";
        private readonly IDictionary<string, string> _flagDescriptions = null;
        private readonly string _commandDescription = "Change the current working directory";
        private readonly IList<string> _exampleUsages = new List<string> { "cd /", "cd ..", "cd home", "cd /home", "cd ../home" };

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Constructors
        public ChangeDirectoryCommand(FileSystemState fileSystemState, DirectoryController directoryController)
        {
            _fileSystemState = fileSystemState;
            _directoryController = directoryController;
        }

        // Class Specific Functionality
        public bool TryValidateArguments(out string responseMessage, params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                responseMessage = $"Error - `{GetCommandName()}` command not initialized correctly";
                return false;
            }

            if (args.FirstOrDefault() != GetCommandName())
            {
                responseMessage = $"Error - Command `{GetCommandName()}` does not match input of `{args.FirstOrDefault()}`";
                return false;
            }

            if (args.Length > 2)
            {
                responseMessage = $"Error - Invalid number of arguments to command `{GetCommandName()}`: {args.Length} arguments";
                return false;
            }

            if (args.Length < 2)
            {
                responseMessage = $"Error - Target directory is required as a parameter for `{GetCommandName()}`";
                return false;
            }

            // User calls `cd <directoryName>` or `cd <directoryPath>/<directoryName>`
            if (args.Length == 2)
            {
                responseMessage = "Command successfully validated";
                return true;
            }

            responseMessage = $"Error - Unexpected validation error - failed to validate command `{GetCommandName()}`";
            return false;
        }

        public string ExecuteCommand(params string[] args)
        {
            // Validate the arguments to the command
            if (!TryValidateArguments(out var validationResponse, args))
            {
                return validationResponse;
            }

            // Execute the valid command logic
            var targetDirectoryName = args.LastOrDefault();

            // Handle the special case where the user wants to change directory to `/` or '\' (root)
            if (targetDirectoryName.Length == 1 && _pathDelimiters.Contains(targetDirectoryName.FirstOrDefault()))
            {
                return ChangeDirectoryToRoot(targetDirectoryName);
            }

            // Handle the special case where the user wants to change directory to `.` (current directory)
            if (targetDirectoryName.Equals(_currentDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                // Do nothing - the user just tried to set the current directory to what it already is
                return "Current working directory changed";
            }

            // Handle the special case where the user wants to change directory to `..` (parent directory)
            if (targetDirectoryName.Equals(_parentDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                return ChangeDirectoryToParent();
            }

            // Handle the special case where the user wants to change directory to `~` (home directory)
            if (targetDirectoryName.Equals(_homeDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                return ChangeDirectoryToHome();
            }

            // Will need to parse the argument to determine if it is a directory or a path to one (fully qualified or relative)
            // We can use the intersection of the delimiter list with the string (list of characters) to determine if any of the characters are a delimiter
            if (targetDirectoryName.Intersect(_pathDelimiters).Any())
            {
                return ChangeDirectoryViaPath(targetDirectoryName);
            }

            // At this point, we know the argument is relative and is a subdirectory of our current directory, so handle that case
            return ChangeDirectoryToSubDirectory(targetDirectoryName);
        }

        private string ChangeDirectoryToRoot(string targetDirectoryName)
        {
            var rootDirectory = _fileSystemState.GetRootDirectory();
            var isRootDirectorySetSuccess = _fileSystemState.TrySetCurrentDirectory(rootDirectory);

            if (!isRootDirectorySetSuccess)
            {
                return $"Error - Failed to set current directory to root `{targetDirectoryName}`";
            }

            return "Current working directory changed";
        }

        private string ChangeDirectoryToParent()
        {
            var currentDirectory = _fileSystemState.GetCurrentDirectory();
            if (currentDirectory.ParentDirectory == null)
            {
                return $"Error - Failed to find parent directory for `{currentDirectory.Name}`";
            }

            var isSetCurrentDirectoryAsParentDirectorySuccess = _fileSystemState.TrySetCurrentDirectory(currentDirectory.ParentDirectory);
            if (!isSetCurrentDirectoryAsParentDirectorySuccess)
            {
                return $"Error - Failed to set current directory to parent directory: `{currentDirectory.ParentDirectory.Name}";
            }

            return "Current working directory changed";
        }

        private string ChangeDirectoryToHome()
        {
            var rootDirectory = _fileSystemState.GetRootDirectory();
            var isGetHomeDirectorySuccess = _directoryController.TryGetDirectory(_homeDirectoryName, rootDirectory, out var homeDirectory);
            if (!isGetHomeDirectorySuccess)
            {
                return $"Error - Failed to get `{_homeDirectoryName}` directory in `{rootDirectory.Name}`";
            }

            var isSetCurrentDirectoryAsHomeSuccess = _fileSystemState.TrySetCurrentDirectory(homeDirectory);
            if (!isSetCurrentDirectoryAsHomeSuccess)
            {
                return $"Error - Failed to set current directory to `{_homeDirectoryName}` directory";
            }

            return $"Current working directory changed";
        }

        private string ChangeDirectoryViaPath(string targetDirectoryPath)
        {
            // There are two options here - first is that this is an absolute path, which means that the first character is a path delimiter
            if (_pathDelimiters.Contains(targetDirectoryPath.FirstOrDefault()))
            {
                return ChangeDirectoryViaAbsolutePath(targetDirectoryPath);
            }

            // Second is that this is a relative path, which means that the current directory is the starting point of the path
            return ChangeDirectoryViaRelativePath(targetDirectoryPath);
        }

        private string ChangeDirectoryViaAbsolutePath(string targetDirectoryPath)
        {
            // Note - When splitting, `/////home` results in just `/home` which is ... weird behavior.  But oddly enough, it's how `cd` normally behaves!
            var targetDirectoryNames = targetDirectoryPath.Split(_pathDelimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var rootDirectory = _fileSystemState.GetRootDirectory();

            // Starting with the root directory, loop through the given directory names until we get to the end with the final target directory (or failure)
            var targetDirectory = rootDirectory;
            foreach (var nextTargetDirectoryName in targetDirectoryNames)
            {
                var isDirectoryRetrievalSuccess = _directoryController.TryGetDirectory(nextTargetDirectoryName, targetDirectory, out var foundDirectory);
                if (!isDirectoryRetrievalSuccess)
                {
                    return $"Error - Failed to find directory named `{nextTargetDirectoryName}` in `{targetDirectory.Name}`";
                }

                targetDirectory = foundDirectory;
            }

            // Now that we have the right target directory, we can try to switch to it
            var isCurrentDirectorySetSuccess = _fileSystemState.TrySetCurrentDirectory(targetDirectory);
            if (!isCurrentDirectorySetSuccess)
            {
                return $"Error - Failed to set current directory to `{targetDirectory.Name}`";
            }

            return "Current working directory changed";
        }

        private string ChangeDirectoryViaRelativePath(string targetDirectoryPath)
        {
            // Note - When splitting, `/////home` results in just `/home` which is ... weird behavior.  But oddly enough, it's how `cd` normally behaves!
            var targetDirectoryNames = targetDirectoryPath.Split(_pathDelimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var currentDirectory = _fileSystemState.GetCurrentDirectory();

            if (targetDirectoryNames.FirstOrDefault() == _homeDirectorySymbol)
            {
                var rootDirectory = _fileSystemState.GetRootDirectory();
                var isGetHomeDirectorySuccess = _directoryController.TryGetDirectory(_homeDirectoryName, rootDirectory, out var homeDirectory);
                if (!isGetHomeDirectorySuccess)
                {
                    return $"Error - Failed to get `{_homeDirectoryName}` directory in `{rootDirectory.Name}`";
                }
            }

            // Starting with the current directory, loop through the given directory names until we get to the end with the final target directory (or failure)
            var targetDirectory = currentDirectory;
            var isHomeDirectorySymbolFirst = false;

            // But first, handle the special case of the first character being the home directory symbol `~`
            if (targetDirectoryNames.FirstOrDefault() == _homeDirectorySymbol)
            {
                isHomeDirectorySymbolFirst = true;

                var rootDirectory = _fileSystemState.GetRootDirectory();
                var isGetHomeDirectorySuccess = _directoryController.TryGetDirectory(_homeDirectoryName, rootDirectory, out targetDirectory);
                if (!isGetHomeDirectorySuccess)
                {
                    return $"Error - Failed to get `{_homeDirectoryName}` directory in `{rootDirectory.Name}`";
                }
            }

            // Okay, now actually loop through and check!
            foreach (var nextTargetDirectoryName in targetDirectoryNames)
            {
                // Since this is a relative path, we could come across special relative path characters, so we should check for those
                // Home directory symbol `~` case (which is only valid if the symbol is the first character, but we have already addressed that)
                if (isHomeDirectorySymbolFirst && nextTargetDirectoryName == _homeDirectorySymbol)
                {
                    continue;
                }

                // Parent directory symbol `..` case
                if (nextTargetDirectoryName == _parentDirectorySymbol)
                {
                    if (targetDirectory.ParentDirectory == null)
                    {
                        return $"Error - Failed to find parent directory for `{targetDirectory.Name}`";
                    }

                    targetDirectory = targetDirectory.ParentDirectory;
                    continue;
                }

                // Current directory symbol `.` case
                if (nextTargetDirectoryName == _currentDirectorySymbol)
                {
                    continue;
                }

                // Regular directory name case
                var isDirectoryRetrievalSuccess = _directoryController.TryGetDirectory(nextTargetDirectoryName, targetDirectory, out var foundDirectory);
                if (!isDirectoryRetrievalSuccess)
                {
                    return $"Error - Failed to find directory named `{nextTargetDirectoryName}` in `{targetDirectory.Name}`";
                }

                targetDirectory = foundDirectory;
            }

            // Now that we have the right target directory, we can try to switch to it
            var isCurrentDirectorySetSuccess = _fileSystemState.TrySetCurrentDirectory(targetDirectory);
            if (!isCurrentDirectorySetSuccess)
            {
                return $"Error - Failed to set current directory to `{targetDirectory.Name}`";
            }

            return "Current working directory changed";
        }

        private string ChangeDirectoryToSubDirectory(string targetDirectoryName)
        {
            var currentDirectory = _fileSystemState.GetCurrentDirectory();
            var isDirectoryRetrievalSuccess = _directoryController.TryGetDirectory(targetDirectoryName, currentDirectory, out var targetDirectory);
            if (!isDirectoryRetrievalSuccess)
            {
                return $"Error - Failed to find directory named `{targetDirectoryName}` in current directory";
            }

            var isCurrentDirectorySetSuccess = _fileSystemState.TrySetCurrentDirectory(targetDirectory);
            if (!isCurrentDirectorySetSuccess)
            {
                return $"Error - Failed to set current directory to `{targetDirectoryName}`";
            }

            return "Current working directory changed";
        }
    }
}
