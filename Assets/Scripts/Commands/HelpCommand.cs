using SysEarth.States;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEarth.Commands
{
    public class HelpCommand : ICommand
    {
        // Class Specific Fields
        private readonly CommandState _commandState;

        // Interface Fields
        private readonly string _commandName = "help";
        private readonly IDictionary<string, string> _flagDescriptions = null;
        private readonly string _commandDescription = "Get a list of the available commands or detailed information for a single command";
        private readonly IList<string> _exampleUsages = new List<string> { "help", "help ls" };

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Constructors
        public HelpCommand(CommandState commandState)
        {
            _commandState = commandState;
        }

        // Class Specific Functionality
        public bool TryValidateArguments(out string responseMessage, params string[] args)
        {
            // In the unique case of the help command as the default command, validation errors are expected and
            // may not even match this command, so first we want to check that the user actually called this command.

            // It is okay if the user did not call this command since it is being treated as a default, so this is technically valid.
            if (args == null || args.Length == 0 || args.FirstOrDefault() != GetCommandName())
            {
                responseMessage = $"`{GetCommandName()}` called by default - Command successfully validated";
                return true;
            }

            // Now we know that the user meant to call the help command, so validate regularly
            if (args.Length > 2)
            {
                responseMessage = $"Error - Invalid number of arguments to command `{GetCommandName()}`: {args.Length} arguments";
                return false;
            }

            // User calls `help`
            if (args.Length == 1)
            {
                responseMessage = "Command successfully validated";
                return true;
            }
            
            // User calls `help <commandName>`
            if (args.Length == 2)
            {
                // We have to validate that the argument passed is a valid command name before we can validate the command
                if (_commandState.TryGetCommand(args.LastOrDefault(), out _))
                {
                    responseMessage = "Command successfully validated";
                    return true;
                }

                responseMessage = $"Error - Command `{args.LastOrDefault()}` not found by `{GetCommandName()}` command.";
                return false;
            }

            responseMessage = $"Error - Unexpected validation error - failed to validate command `{GetCommandName()}`";
            return false;
        }

        public string ExecuteCommand(params string[] args)
        {
            var responseMessage = new StringBuilder();

            // Validate the arguments to the command
            var isCommandValid = TryValidateArguments(out var validationResponse, args);
            if (!isCommandValid)
            {
                responseMessage.AppendLine(validationResponse);
            }

            // Execute the valid command logic
            // `help <CommandName>` execution
            var isArgsValidForSingleCommandHelp = args != null && args.Length == 2 && args.FirstOrDefault() == GetCommandName();
            if (isCommandValid && isArgsValidForSingleCommandHelp && _commandState.TryGetCommand(args.LastOrDefault(), out var command))
            {
                responseMessage.AppendLine($"Command: {command.GetCommandName()}");
                responseMessage.AppendLine($"Description: {command.GetCommandDescription()}");

                var flagDescriptions = command.GetCommandFlagDescriptions();
                if (flagDescriptions != null && flagDescriptions.Any())
                {
                    responseMessage.AppendLine("Options:");
                    foreach (var flagDescription in flagDescriptions)
                    {
                        responseMessage.AppendLine($"  {flagDescription.Key} : {flagDescription.Value}");
                    }
                }

                var exampleUsages = command.GetExampleUsages();
                if (exampleUsages != null && exampleUsages.Any())
                {
                    responseMessage.AppendLine("Example Usages:");
                    foreach (var exampleUsage in exampleUsages)
                    {
                        responseMessage.AppendLine($"  {exampleUsage}");
                    }
                }
            }

            // Default - `help` execution
            else
            {
                var availableCommands = _commandState.GetAvailableCommands().OrderBy(x => x); // Order alphabetically
                responseMessage.AppendLine("Available Commands:");

                // Build the list of available commands to inform the user of them
                foreach (var availableCommand in availableCommands)
                {
                    responseMessage.AppendLine(availableCommand);
                }
            }

            return responseMessage.ToString();
        }
    }
}
