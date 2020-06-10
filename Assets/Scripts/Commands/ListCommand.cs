using SysEarth.Controllers;
using SysEarth.Models;
using SysEarth.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEarth.Commands
{
    public class ListCommand : ICommand
    {
        // Class Specific Constants
        private const string _directoryIndicator = "/";
        private const string _currentDirectorySymbol = ".";
        private const string _parentDirectorySymbol = "..";

        private const char _hiddenFileStartingCharacter = '.';
        private const char _flagParameterStartingCharacter = '-';
        private const char _showHiddenFilesFlag = 'a';

        private readonly IList<char> _pathDelimiters = new List<char> { '\\', '/' };

        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;
        private readonly DirectoryController _directoryController;

        // Interface Fields
        private readonly string _commandName = "ls";
        private readonly string _commandDescription = "List the file contents of a target directory";
        private readonly IDictionary<string, string> _flagDescriptions = new Dictionary<string, string>
        {
            { $"{_flagParameterStartingCharacter}{_showHiddenFilesFlag}", "Show all files, including hidden ones that start with `.`" }
        };
        private readonly IList<string> _exampleUsages = new List<string> { "ls", "ls -a", "ls /", "ls ..", "ls home", "ls -a home", "ls /home", "ls ../home" };

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Constructors
        public ListCommand(FileSystemState fileSystemState, DirectoryController directoryController)
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

            // If the user specifies more than one target directory (or non-flag) input, the input is invalid
            int targetDirectoryCounter = 0;
            foreach (var arg in args)
            {
                if (arg == args.FirstOrDefault()) continue;

                else if (arg.StartsWith(_flagParameterStartingCharacter.ToString()))
                {
                    continue;
                }

                else
                {
                    targetDirectoryCounter++;
                }
            }

            if (targetDirectoryCounter > 1)
            {
                responseMessage = $"Error - More than one target directory specified as input to `{GetCommandName()}`";
                return false;
            }

            // User calls `ls [-<flags>] <directoryName>` or `ls [-<flags>] <directoryPath>/<directoryName>`
            if (targetDirectoryCounter == 1)
            {
                responseMessage = "Command successfully validated";
                return true;
            }

            // User calls `ls [-<flags>]`
            if (targetDirectoryCounter == 0)
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

            // Extract the arguments and flags
            string targetDirectoryName = null;
            bool showHiddenFiles = false;

            // Ignore the first parameter because we know it is the command name since validation succeeded
            for (var i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith(_flagParameterStartingCharacter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // Loop through all the flags provided like `-abcdefg` and extract each one as necessary
                    var shortOptions = args[i];
                    shortOptions = shortOptions.TrimStart(_flagParameterStartingCharacter);
                    foreach (var shortOption in shortOptions)
                    {
                        if (shortOption.Equals(_showHiddenFilesFlag))
                        {
                            showHiddenFiles = true;
                        }
                        else
                        {
                            return $"Error - Unknown flag provided to `{GetCommandName()}`: `{shortOption}`";
                        }
                    }
                }
                else
                {
                    targetDirectoryName = args[i];
                }
            }

            // Execute the valid command logic
            Directory targetDirectory = null;
            IEnumerable<string> directoryContents = null;

            // Handle the special case where the user wants to list the contents of the `/` or '\' directory (root)
            if (targetDirectoryName != null && targetDirectoryName.Length == 1 && _pathDelimiters.Contains(targetDirectoryName.FirstOrDefault()))
            {
                targetDirectory = _fileSystemState.GetRootDirectory();
                directoryContents = GetTargetDirectoryContents(targetDirectory);
            }

            // Handle the special case where the user wants to list the contents of the `.` directory (current directory)
            if (targetDirectoryName != null && targetDirectoryName.Equals(_currentDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                targetDirectory = _fileSystemState.GetCurrentDirectory();
                directoryContents = GetTargetDirectoryContents(targetDirectory);
            }

            // Handle the special case where the user wants to change directory to `..` (parent directory)
            if (targetDirectoryName != null && targetDirectoryName.Equals(_parentDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                var currentDirectory = _fileSystemState.GetCurrentDirectory();
                targetDirectory = currentDirectory.ParentDirectory;
                if (targetDirectory == null)
                {
                    return $"Error - Failed to find parent directory for `{currentDirectory.Name}`";
                }

                directoryContents = GetTargetDirectoryContents(targetDirectory);
            }

            // Handle the special case where the user has supplied a directory name or path they wish to see the contents of
            if (targetDirectoryName != null)
            {
                var isGetDirectorySuccess = TryGetTargetDirectory(targetDirectoryName, out targetDirectory, out var errorMessage);
                if (!isGetDirectorySuccess)
                {
                    return errorMessage;
                }

                directoryContents = GetTargetDirectoryContents(targetDirectory);
            }

            // At this point, if we do not already have contents built, the user did not specify a target directory to see its contents
            // If that is the case, use the "default" target directory - the current working directory
            if (directoryContents == null)
            {
                targetDirectory = _fileSystemState.GetCurrentDirectory();
                directoryContents = GetTargetDirectoryContents(targetDirectory);
            }

            // Now that we have directory contents, if the user wants to see hidden files, simply show them the contents unedited
            // But add `.` and `..` to the list of sub directory names implicitly (even if they are a wrapper of sorts)
            if (showHiddenFiles)
            {
                directoryContents = AddSymbolsToDirectoryContents(targetDirectory, directoryContents);
            }

            // If the user did not specify they want to see hidden files, filter them out of the file list
            else
            {
                directoryContents = RemoveHiddenFilesFromDirectoryContents(directoryContents);
            }

            return FormatDirectoryContents(targetDirectory.Name, directoryContents);
        }

        private bool TryGetTargetDirectory(string targetDirectoryPath, out Directory targetDirectory, out string errorMessage)
        {
            // There are two options here - first is that this is an absolute path, which means that the first character is a path delimiter
            if (_pathDelimiters.Contains(targetDirectoryPath.FirstOrDefault()))
            {
                var rootDirectory = _fileSystemState.GetRootDirectory();
                return TryGetTargetDirectory(rootDirectory, targetDirectoryPath, out targetDirectory, out errorMessage);
            }

            // Second is that this is a relative path, which means that the current directory is the starting point of the path
            var currentDirectory = _fileSystemState.GetCurrentDirectory();
            return TryGetTargetDirectory(currentDirectory, targetDirectoryPath, out targetDirectory, out errorMessage);
        }

        private bool TryGetTargetDirectory(Directory startingDirectory, string targetDirectoryPath, out Directory targetDirectory, out string errorMessage)
        {
            // Note - When splitting, `/////home` results in just `/home` which is ... weird behavior.  But oddly enough, it's how `ls` normally behaves!
            var targetDirectoryNames = targetDirectoryPath.Split(_pathDelimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            // Starting with the root directory, loop through the given directory names until we get to the end with the final target directory (or failure)
            targetDirectory = startingDirectory;
            foreach (var nextTargetDirectoryName in targetDirectoryNames)
            {
                var isDirectoryRetrievalSuccess = _directoryController.TryGetDirectory(nextTargetDirectoryName, targetDirectory, out var foundDirectory);
                if (!isDirectoryRetrievalSuccess)
                {
                    errorMessage = $"Error - Failed to find directory named `{nextTargetDirectoryName}` in `{targetDirectory.Name}`";
                    return false;
                }

                targetDirectory = foundDirectory;
            }

            errorMessage = null;
            return true;
        }

        private IEnumerable<string> GetTargetDirectoryContents(Directory targetDirectory)
        {
            // Build a list of all the files and sub-directories in the target directory
            var fileNames = targetDirectory.FilesInDirectory.Select(x => x.Name);
            var subDirectoryNames = targetDirectory.SubDirectories.Select(x => x.Name + _directoryIndicator);

            // Zip together all of the files and subdirectories in this directory
            return fileNames.Concat(subDirectoryNames);
        }

        private IEnumerable<string> AddSymbolsToDirectoryContents(Directory targetDirectory, IEnumerable<string> currentDirectoryContents)
        {
            var symbolDirectories = new List<string>
            {
                _currentDirectorySymbol + _directoryIndicator
            };

            // Only want to add the parent directory symbo if a parent directory exists
            if (targetDirectory.ParentDirectory != null)
            {
                symbolDirectories.Add(_parentDirectorySymbol + _directoryIndicator);
            }

            return symbolDirectories.Concat(currentDirectoryContents);
        }

        private IEnumerable<string> RemoveHiddenFilesFromDirectoryContents(IEnumerable<string> currentDirectoryContents)
        {
            return currentDirectoryContents.Where(x => x.StartsWith(_hiddenFileStartingCharacter.ToString()));
        }

        private string FormatDirectoryContents(string directoryName, IEnumerable<string> directoryContents)
        {
            // Build an output list of all the files and sub-directories in the target directory
            var responseMessage = new StringBuilder();
            responseMessage.AppendLine($"Contents of `{directoryName}` Directory:");

            // Order all of the files and folder so that they can be displayed alphabetically
            directoryContents = directoryContents.OrderBy(x => x);

            foreach (var itemInDirectory in directoryContents)
            {
                responseMessage.AppendLine(itemInDirectory);
            }

            return responseMessage.ToString();
        }
    }
}
