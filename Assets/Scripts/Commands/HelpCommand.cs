using SysEarth.States;
using System.Collections.Generic;
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
            // In the unique case of the help command as the default command, validation errors are expected and
            // may not even match this command, so we can skip validation entirely.
            responseMessage = "Successfully validated";
            return true;
        }

        public string ExecuteCommand(params string[] args)
        {
            var responseMessage = new StringBuilder();

            // Validate the arguments to the command
            if (!TryValidateArguments(out var validationResponse, args))
            {
                responseMessage.AppendLine(validationResponse);
            }

            // Execute the valid command logic
            var availableCommands = _commandState.GetAvailableCommands();
            responseMessage.AppendLine("Available Commands:");

            // Build the list of available commands to inform the user of them
            foreach (var command in availableCommands)
            {
                responseMessage.AppendLine(command);
            }

            return responseMessage.ToString();
        }
    }
}
