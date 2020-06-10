using SysEarth.Commands;
using SysEarth.States;
using UnityEngine;

namespace SysEarth.Controllers
{
    public class CommandController
    {
        public bool TryGetCommand(CommandState commandState, string commandName, out ICommand command)
        {
            // Try to get the command that the user entered
            if (commandState.TryGetCommand(commandName, out command))
            {
                return true;
            }

            Debug.Log($"Warning - Command `{commandName}` not found");
            return false;
        }
    }
}
