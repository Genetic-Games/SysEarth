using SysEarth.Controllers;
using SysEarth.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Commands
{
    public class ChangeDirectoryCommand : ICommand
    {
        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;
        private readonly DirectoryController _directoryController;

        // Interface Fields
        private readonly string _commandName = "cd";
        private readonly IDictionary<string, string> _flagDescriptions = null;
        private readonly string _commandDescription = "Change the current working directory";
        private readonly IList<string> _exampleUsages = new List<string> { "cd /", "cd ..", "cd home" };

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

            // User calls `cd <directoryName>`
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

            // Handle the special case where the user wants to change directory to `/` (root)
            if (targetDirectoryName.Equals("/", StringComparison.InvariantCultureIgnoreCase))
            {
                return ChangeDirectoryToRoot(targetDirectoryName);
            }

            // Handle the special case where the user wants to change directory to `.` (current directory)
            if (targetDirectoryName.Equals(".", StringComparison.InvariantCultureIgnoreCase))
            {
                // Do nothing - the user just tried to set the current directory to what it already is
                return "Current working directory changed";
            }

            // Handle the special case where the user wants to change directory to `..` (parent directory)
            if (targetDirectoryName.Equals("..", StringComparison.InvariantCultureIgnoreCase))
            {
                return ChangeDirectoryToParent(targetDirectoryName);
            }

            // Will need to parse the argument to determine if there it is a directory or a path to one (fully qualified or relative)
            // TODO - Parse the argument to do this

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

        private string ChangeDirectoryToParent(string targetDirectoryName)
        {
            var currentDirectory = _fileSystemState.GetCurrentDirectory();
            if (currentDirectory.ParentDirectory == null)
            {
                return $"Error - Failed to find parent directory for `{currentDirectory.Name}`";
            }

            var isParentDirectorySetSuccess = _fileSystemState.TrySetCurrentDirectory(currentDirectory.ParentDirectory);
            if (!isParentDirectorySetSuccess)
            {
                return $"Error - Failed to set current directory to parent directory: `{currentDirectory.ParentDirectory.Name}";
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
