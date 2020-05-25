using SysEarth.Commands;
using SysEarth.States;
using UnityEngine;

namespace SysEarth.Controllers
{
    public class CommandController
    {
        private string _defaultCommand = "help";

        public ICommand GetCommand(CommandState commandState, string commandName)
        {
            // Try to get the command that the user entered
            if (commandState.TryGetCommand(commandName, out var command))
            {
                return command;
            }

            // Default case is the `help` command if we did not find a match with other commands
            if (!commandState.TryGetCommand(_defaultCommand, out var defaultCommand))
            {
                Debug.Log($"Error - Failed to retrieve default command: `{_defaultCommand}`");
            }

            return defaultCommand;
        }
    }
}
