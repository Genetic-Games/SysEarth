using SysEarth.Commands;
using SysEarth.States;
using UnityEngine;

namespace SysEarth.Controllers
{
    public class CommandController
    {
        private string _defaultCommand = "help";

        public bool TryGetCommand(CommandState commandState, string commandName, out ICommand command)
        {
            // Try to get the command that the user entered
            if (commandState.TryGetCommand(commandName, out command))
            {
                return true;
            }

            Debug.Log($"Warning - Command `{commandName}` not found");

            // Default case is the `help` command if we did not find a match with other commands
            if (!commandState.TryGetCommand(_defaultCommand, out command))
            {
                Debug.Log($"Error - Failed to retrieve default command: `{_defaultCommand}`");
            }

            return false;
        }
    }
}
