using SysEarth.Commands;
using SysEarth.Controllers;
using SysEarth.States;
using System.Collections.Generic;
using UnityEngine;

namespace SysEarth.Initializers
{
    public class CommandStateInitializer
    {
        public IList<string> InitializeCommandState(
            CommandState commandState,
            TerminalState terminalState,
            FileSystemState fileSystemState,
            DirectoryController directoryController)
        {
            var commandsInitialized = new List<string>();

            var helpCommand = new HelpCommand(commandState);
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(helpCommand.GetCommandName(), helpCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `help` command to available command state");
            commandsInitialized.Add(helpCommand.GetCommandName());

            var clearCommand = new ClearCommand(terminalState);
            isAddCommandSuccess = commandState.TryAddAvailableCommand(clearCommand.GetCommandName(), clearCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `clear` command to available command state");
            commandsInitialized.Add(clearCommand.GetCommandName());

            var listCommand = new ListCommand(fileSystemState, directoryController);
            isAddCommandSuccess = commandState.TryAddAvailableCommand(listCommand.GetCommandName(), listCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `ls` command to available command state");
            commandsInitialized.Add(listCommand.GetCommandName());

            var changeDirectoryCommand = new ChangeDirectoryCommand(fileSystemState, directoryController);
            isAddCommandSuccess = commandState.TryAddAvailableCommand(changeDirectoryCommand.GetCommandName(), changeDirectoryCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `cd` command to available command state");
            commandsInitialized.Add(changeDirectoryCommand.GetCommandName());

            return commandsInitialized;
        }
    }
}
