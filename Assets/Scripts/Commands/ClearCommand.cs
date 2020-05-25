using SysEarth.States;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Commands
{
    public class ClearCommand : ICommand
    {
        // Class Specific Fields
        private readonly TerminalState _terminalState;

        // Interface Fields
        private readonly string _commandName = "clear";
        private IDictionary<string, string> _flagDescriptions = null;
        private string _commandDescription = "Clear the console of text";
        private IList<string> _exampleUsages = new List<string> { "clear" };

        // Interface Properties
        public string GetCommandName() => _commandName;
        public string GetCommandDescription() => _commandDescription;
        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;
        public IList<string> GetExampleUsages() => _exampleUsages;

        // Constructors
        public ClearCommand(TerminalState terminalState)
        {
            _terminalState = terminalState;
        }

        // Class Specific Functionality
        public bool TryValidateArguments(out string responseMessage, params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                responseMessage = $"Error - `{GetCommandName()}` command not initialized correctly";
                return false;
            }

            if (args.Length >= 2)
            {
                responseMessage = $"Error - Invalid number of arguments to command `{GetCommandName()}`: {args.Length} arguments";
                return false;
            }

            if (args.FirstOrDefault() != GetCommandName())
            {
                responseMessage = $"Error - Command `{GetCommandName()}` does not match input of `{args.FirstOrDefault()}`";
                return false;
            }

            // User calls `clear`
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
            _terminalState.HidePreviousCommands();
            return "Console cleared";
        }
    }
}
