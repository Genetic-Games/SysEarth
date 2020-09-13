using SysEarth.Controllers;
using SysEarth.States;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Commands
{
    public class InitializePlanetCommand : ICommand
    {
        // Interface Fields
        private readonly string _commandName = "init";
        private readonly IDictionary<string, string> _flagDescriptions = null;
        private readonly string _commandDescription = "Initialize a new planet to terraform";
        private readonly IList<string> _exampleUsages = new List<string> { "init earth", "init planetName" };

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Class Specific Fields
        private readonly FileSystemState _fileSystemState;
        private readonly DirectoryController _directoryController;
        private readonly PermissionController _permissionController;

        // Constructors
        public InitializePlanetCommand(FileSystemState fileSystemState, DirectoryController directoryController, PermissionController permissionController)
        {
            _fileSystemState = fileSystemState;
            _directoryController = directoryController;
            _permissionController = permissionController;
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
                responseMessage = $"Error - Planet name is required as a parameter for `{GetCommandName()}`";
                return false;
            }

            // User calls `init <planetName>`
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
            var targetPlanetName = args.LastOrDefault();

            // Try to initialize a new directory for this planet
            var rootDirectory = _fileSystemState.GetRootDirectory();
            var planetPermission = _permissionController.GetCustomPermission(canRead: true, canExecute: true);

            var isAddPlanetDirectorySuccess = _directoryController.TryAddDirectory(targetPlanetName, planetPermission, rootDirectory, out var targetPlanetDirectory);
            if (!isAddPlanetDirectorySuccess)
            {
                return $"Error - Failed to initialize planet `{targetPlanetName}`";
            }

            // Once we know we have a planet folder initialized, set it up with all the needed sub-directories for a planet
            // TODO - Setup the sub-directories for a planet

            return null;
        }
    }
}
