using SysEarth.Commands;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.States
{
    public class CommandState
    {
        private IDictionary<string, ICommand> _availableCommands;

        public CommandState()
        {
            _availableCommands = new Dictionary<string, ICommand>();
        }

        public IList<string> GetAvailableCommands()
        {
            return _availableCommands.Keys.ToList();
        }

        public bool IsCommandAvailable(string commandName)
        {
            return _availableCommands.Keys.Contains(NormalizeCommandName(commandName));
        }

        public bool TryGetCommand(string commandName, out ICommand command)
        {
            return _availableCommands.TryGetValue(NormalizeCommandName(commandName), out command);
        }

        public bool TryAddAvailableCommand(string commandName, ICommand command)
        {
            if (string.IsNullOrEmpty(commandName) || command == null || IsCommandAvailable(commandName))
            {
                return false;
            }

            _availableCommands.Add(NormalizeCommandName(commandName), command);
            return true;
        }

        public bool TryRemoveAvailableCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName) || !IsCommandAvailable(commandName))
            {
                return false;
            }

            return _availableCommands.Remove(NormalizeCommandName(commandName));
        }

        /// <summary>
        /// Helper function to ensure that all commands in the set are normalized (not case sensitive, etc)
        /// </summary>
        private string NormalizeCommandName(string commandName)
        {
            return commandName?.ToLowerInvariant() ?? string.Empty;
        }
    }
}
