using SysEarth.States;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEarth.Commands
{
    public class ListCommand : ICommand
    {
        private const string _directoryIndicator = "/";

        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;

        // Interface Fields
        private readonly string _commandName = "ls";
        private readonly IDictionary<string, string> _flagDescriptions = null; // TODO - Add flags for ls
        private readonly string _commandDescription = "List the file contents of a target directory";
        private readonly IList<string> _exampleUsages = new List<string> { "ls" }; // TODO - Add more example usages with flags

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Constructors
        public ListCommand(FileSystemState fileSystemState)
        {
            _fileSystemState = fileSystemState;
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

            // TODO - Change this validation so that the second parameter can be a file name if we do not want to run `ls` in current directory (or want flags!)
            if (args.Length >= 2)
            {
                responseMessage = $"Error - Invalid number of arguments to command `{GetCommandName()}`: {args.Length} arguments";
                return false;
            }

            // User calls `ls`
            if (args.Length == 1)
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
            // TODO - Currently assumes current directory but could easily target a different directory as a parameter
            var currentDirectory = _fileSystemState.GetCurrentDirectory();

            // Build a list of all the files and sub-directories in the target directory
            var responseMessage = new StringBuilder();
            responseMessage.AppendLine($"Contents of `{currentDirectory.Name}` Directory:");

            var fileNames = currentDirectory.FilesInDirectory.Select(x => x.Name);
            var subDirectoryNames = currentDirectory.SubDirectories.Select(x => x.Name + _directoryIndicator).ToList();

            // Add `.` and `..` to the list of sub directory names implicitly (even if they are a wrapper of sorts)
            subDirectoryNames.Add(".");
            subDirectoryNames.Add("..");

            // Zip together all of the files and folder so that they can be displayed alphabetically
            var directoryContents = fileNames.Concat(subDirectoryNames).OrderBy(x => x);

            foreach (var itemInDirectory in directoryContents)
            {
                responseMessage.AppendLine(itemInDirectory);
            }

            return responseMessage.ToString();
        }
    }
}
