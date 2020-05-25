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
        private IDictionary<string, string> _flagDescriptions = null;
        private string _commandDescription = "Get information about the commands available";
        private IList<string> _exampleUsages = new List<string> { "help" };

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

            // User calls `help`
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
            var responseMessage = new StringBuilder();

            // In the unique case of the help command as the default command, validation errors are expected
            // Instead of returning on failed validation, we add it to the output to inform the user of the issue
            if (!TryValidateArguments(out var validationResponse, args))
            {
                responseMessage.AppendLine(validationResponse);
            }

            // Build the list of available commands to inform the user
            var availableCommands = _commandState.GetAvailableCommands();
            responseMessage.AppendLine("Available Commands:");

            foreach (var command in availableCommands)
            {
                responseMessage.AppendLine(command);
            }

            return responseMessage.ToString();
        }
    }
}
