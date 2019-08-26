using System;
using System.Collections.Generic;

namespace SysEarth.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly string _commandName = "help";

        private IDictionary<string, string> _flagDescriptions = null;

        private string _commandDescription = "Get information about the commands available";

        private IList<string> _exampleUsages = new List<string> { "help" };

        private IList<string> _responseMessages = new List<string> { "Command not yet validated or executed." };

        public string GetCommandName() => _commandName;

        public string GetCommandDescription() => _commandDescription;

        public IDictionary<string, string> GetCommandFlagDescriptions() => _flagDescriptions;

        public IList<string> GetExampleUsages() => _exampleUsages;

        public IList<string> GetResponseMessages() => _responseMessages;

        public bool ValidateArguments(IList<string> args)
        {
            _responseMessages = new List<string>();

            if (args == null || args.Count == 0)
            {
                _responseMessages.Add($"`{_commandName}` command not initialized correctly");
                return false;
            }

            if (args.Count >= 2)
            {
                _responseMessages.Add($"Invalid number of arguments: {args.Count}");
                return false;
            }

            if (args[0] != _commandName) // TODO - find a safer way than array access
            {
                _responseMessages.Add($"Command name of `{_commandName}` does not match input of `{args[0]}`");
                return false;
            }

            // User calls `help`
            if (args.Count == 1) 
            {
                _responseMessages.Add("Successfully validated");
                return true;
            }

            return false;
        }

        public void ExecuteCommand(IList<string> args)
        {
            if (args == null)
            {
                throw new ArgumentNullException($"`{_commandName}` command not initialized correctly");
            }

            bool isValidArguments = ValidateArguments(args);

            if (!isValidArguments)
            {
                throw new InvalidOperationException($"Invalid arguments to `{_commandName}` command");
            }

            // TODO - return list of all valid commands that should live ... somewhere?
            throw new NotImplementedException();
        }
    }
}
