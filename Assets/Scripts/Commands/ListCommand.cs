using SysEarth.Controllers;
using SysEarth.Enums;
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
        private const string _hiddenFileIndicator = ".";

        private const string _currentDirectorySymbol = ".";
        private const string _parentDirectorySymbol = "..";
        private const string _homeDirectorySymbol = "~";

        private const char _flagParameterStartingCharacter = '-';
        private const char _showHiddenFilesFlag = 'a';
        private const char _showFileListViewFlag = 'l';

        private const string _readPermissionString = "R";
        private const string _writePermissionString = "W";
        private const string _executePermissionString = "X";
        private const string _noPermissionString = "-";

        private readonly IList<char> _pathDelimiters = new List<char> { '\\', '/' };

        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;
        private readonly DirectoryController _directoryController;

        // Interface Fields
        private readonly string _commandName = "ls";
        private readonly string _commandDescription = "List the file contents of a target directory";
        private readonly IDictionary<string, string> _flagDescriptions = new Dictionary<string, string>
        {
            { $"{_flagParameterStartingCharacter}{_showHiddenFilesFlag}", $"Show all files, including hidden ones that start with `{_hiddenFileIndicator}`" },
            { $"{_flagParameterStartingCharacter}{_showFileListViewFlag}", "Show long list format of directory contents, including file permissions" }
        };
        private readonly IList<string> _exampleUsages = new List<string> { "ls", "ls -al", "ls /", "ls ..", "ls home", "ls -a home", "ls /home", "ls -l ../home" };

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
            bool showFileListView = false;

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
                        else if (shortOption.Equals(_showFileListViewFlag))
                        {
                            showFileListView = true;
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

            // Handle the special case where the user wants to list the contents of the `/` or '\' directory (root)
            if (targetDirectoryName != null && targetDirectoryName.Length == 1 && _pathDelimiters.Contains(targetDirectoryName.FirstOrDefault()))
            {
                targetDirectory = _fileSystemState.GetRootDirectory();
            }

            // Handle the special case where the user wants to list the contents of the `.` directory (current directory)
            if (targetDirectoryName != null && targetDirectoryName.Equals(_currentDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                targetDirectory = _fileSystemState.GetCurrentDirectory();
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
            }

            // Handle the special case where the user has supplied a directory name or path they wish to see the contents of
            if (targetDirectoryName != null)
            {
                var isGetDirectorySuccess = TryGetTargetDirectory(targetDirectoryName, out targetDirectory, out var errorMessage);
                if (!isGetDirectorySuccess)
                {
                    return errorMessage;
                }
            }

            // At this point, if we do not already have a target directory, the user did not specify a target directory to see its contents
            // If that is the case, use the "default" target directory - the current working directory
            if (targetDirectory == null)
            {
                targetDirectory = _fileSystemState.GetCurrentDirectory();
            }
            
            // If the user specified that they want to see the long list view, show them that view
            if (showFileListView)
            {
                return FormatDirectoryContentsForLongView(targetDirectory, showHiddenFiles);
            }

            // Otherwise, show them the default view
            return FormatDirectoryContents(targetDirectory, showHiddenFiles);
        }

        private bool TryGetTargetDirectory(string targetDirectoryPath, out Directory targetDirectory, out string errorMessage)
        {
            // There are three options here - first is that this is an absolute path, which means that the first character is a path delimiter
            if (_pathDelimiters.Contains(targetDirectoryPath.FirstOrDefault()))
            {
                var rootDirectory = _fileSystemState.GetRootDirectory();
                return TryGetTargetDirectory(rootDirectory, targetDirectoryPath, out targetDirectory, out errorMessage);
            }

            // Second is that this is a relative path, but begins with a `~` symbol, which means the home directory is the starting point of the path
            if (_homeDirectorySymbol.Equals(targetDirectoryPath.FirstOrDefault().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                var rootDirectory = _fileSystemState.GetRootDirectory();
                var isGetHomeDirectorySuccess = _directoryController.TryGetDirectory("home", rootDirectory, out var homeDirectory);
                return TryGetTargetDirectory(homeDirectory, targetDirectoryPath, out targetDirectory, out errorMessage, ignoreFirstHomeSymbol: true);
            }

            // Third is that this is a relative path that does not begin with a `~` symbol, which means that the current directory is the starting point of the path
            var currentDirectory = _fileSystemState.GetCurrentDirectory();
            return TryGetTargetDirectory(currentDirectory, targetDirectoryPath, out targetDirectory, out errorMessage);
        }

        private bool TryGetTargetDirectory(Directory startingDirectory, string targetDirectoryPath, out Directory targetDirectory, out string errorMessage, bool ignoreFirstHomeSymbol = false)
        {
            // Note - When splitting, `/////home` results in just `/home` which is ... weird behavior.  But oddly enough, it's how `ls` normally behaves!
            var targetDirectoryNames = targetDirectoryPath.Split(_pathDelimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            // Manually ignore the first character if it is a home symbol `~` since it has already been addressed with the starting directory
            if (ignoreFirstHomeSymbol && targetDirectoryNames.FirstOrDefault().Equals(_homeDirectorySymbol, StringComparison.InvariantCultureIgnoreCase))
            {
                targetDirectoryNames.RemoveAt(0);
            }

            // Starting at the passed in directory, loop through the given directory names until we get to the end with the final target directory (or failure)
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

        private string GetAccessString(Permission access)
        {
            var read = access.Read ? _readPermissionString : _noPermissionString;
            var write = access.Write ? _writePermissionString : _noPermissionString;
            var execute = access.Execute ? _executePermissionString : _noPermissionString;

            return $"{read} {write} {execute}";
        }

        private string FormatDirectoryContentsForLongView(Directory targetDirectory, bool showHiddenFiles)
        {
            // Build a long view output list of all the files and sub-directories in the target directory with their permissions
            var responseMessage = new StringBuilder();
            responseMessage.AppendLine($"Contents of `{targetDirectory.Name}` Directory:");

            // Build a sorted dictionary of item name to output string for that item so we can easily build the response in order later
            var directoryContents = new SortedDictionary<string, string>();
            foreach (var fileInDirectory in targetDirectory.FilesInDirectory)
            {
                // If the user did not specify seeing hidden files and the file name starts with a hidden character, hide it from the user
                if (showHiddenFiles || !fileInDirectory.Name.StartsWith(_hiddenFileIndicator))
                {
                    var fileExtension = fileInDirectory.Extension == FileExtension.None
                        ? string.Empty
                        : $".{fileInDirectory.Extension.ToString()}";
                    directoryContents.Add(fileInDirectory.Name, $"{GetAccessString(fileInDirectory.Access)}   {fileInDirectory.Name}{fileExtension}");
                }
            }

            foreach (var subDirectoryInDirectory in targetDirectory.SubDirectories)
            {
                // If the user did not specify seeing hidden files and the file name starts with a hidden character, hide it from the user
                if (showHiddenFiles || !subDirectoryInDirectory.Name.StartsWith(_hiddenFileIndicator))
                {
                    directoryContents.Add(subDirectoryInDirectory.Name, $"{GetAccessString(subDirectoryInDirectory.Access)}   {subDirectoryInDirectory.Name}{_directoryIndicator}");
                }
            }

            // If the user wants to see hidden files, add `.` and `..` to the list of sub directory names implicitly
            if (showHiddenFiles)
            {
                directoryContents = AddSymbolsToDirectoryContentsForLongView(targetDirectory, directoryContents);
            }

            // Build the response knowing that the the items are ordered ordering by key (which is file / folder name)
            foreach (var itemInDirectory in directoryContents.Values)
            {
                responseMessage.AppendLine(itemInDirectory);
            }

            return responseMessage.ToString();
        }

        private string FormatDirectoryContents(Directory targetDirectory, bool showHiddenFiles)
        {
            // Build an output list of all the files and sub-directories in the target directory
            var responseMessage = new StringBuilder();
            responseMessage.AppendLine($"Contents of `{targetDirectory.Name}` Directory:");

            // Build a sorted dictionary of item name to output string for that item so we can easily build the response in order later
            var directoryContents = new SortedDictionary<string, string>();
            foreach (var fileInDirectory in targetDirectory.FilesInDirectory)
            {
                // If the user did not specify seeing hidden files and the file name starts with a hidden character, hide it from the user
                if (showHiddenFiles || !fileInDirectory.Name.StartsWith(_hiddenFileIndicator))
                {
                    var fileExtension = fileInDirectory.Extension == FileExtension.None
                        ? string.Empty
                        : $".{fileInDirectory.Extension.ToString()}";
                    directoryContents.Add(fileInDirectory.Name, $"{fileInDirectory.Name}{fileExtension}");
                }
            }

            foreach (var subDirectoryInDirectory in targetDirectory.SubDirectories)
            {
                // If the user did not specify seeing hidden files and the file name starts with a hidden character, hide it from the user
                if (showHiddenFiles || !subDirectoryInDirectory.Name.StartsWith(_hiddenFileIndicator))
                {
                    directoryContents.Add(subDirectoryInDirectory.Name, $"{subDirectoryInDirectory.Name}{_directoryIndicator}");
                }
            }

            // If the user wants to see hidden files, add `.` and `..` to the list of sub directory names implicitly
            if (showHiddenFiles)
            {
                directoryContents = AddSymbolsToDirectoryContents(targetDirectory, directoryContents);
            }

            // Build the response knowing that the the items are ordered ordering by key (which is file / folder name)
            foreach (var itemInDirectory in directoryContents.Values)
            {
                responseMessage.AppendLine(itemInDirectory);
            }

            return responseMessage.ToString();
        }

        private SortedDictionary<string, string> AddSymbolsToDirectoryContents(Directory targetDirectory, SortedDictionary<string, string> currentDirectoryContents)
        {
            currentDirectoryContents.Add($"{_currentDirectorySymbol}{_directoryIndicator}", $"{_currentDirectorySymbol}{_directoryIndicator}");

            // Only want to add the parent directory symbol if a parent directory exists
            if (targetDirectory.ParentDirectory != null)
            {
                currentDirectoryContents.Add($"{_parentDirectorySymbol}{_directoryIndicator}", $"{_parentDirectorySymbol}{_directoryIndicator}");
            }

            return currentDirectoryContents;
        }

        private SortedDictionary<string, string> AddSymbolsToDirectoryContentsForLongView(Directory targetDirectory, SortedDictionary<string, string> currentDirectoryContents)
        {
            currentDirectoryContents.Add($"{_currentDirectorySymbol}", $"{GetAccessString(targetDirectory.Access)}   {_currentDirectorySymbol}{_directoryIndicator}");

            // Only want to add the parent directory symbol if a parent directory exists
            if (targetDirectory.ParentDirectory != null)
            {
                currentDirectoryContents.Add($"{_parentDirectorySymbol}", $"{GetAccessString(targetDirectory.ParentDirectory.Access)}   {_parentDirectorySymbol}{_directoryIndicator}");
            }

            return currentDirectoryContents;
        }
    }
}
