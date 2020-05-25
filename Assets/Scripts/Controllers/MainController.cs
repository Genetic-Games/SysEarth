using SysEarth.Commands;
using SysEarth.Models;
using SysEarth.States;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class MainController : MonoBehaviour
    {
        // Scene References
        public Text InputTextObject;
        public Text OutputTextObject;

        // Input Variables
        public int InputLengthCharacterLimit = 100;
        public int InputHistoryLimit = 10;

        // States
        private FileSystemState _fileSystemState;
        private TerminalState _terminalState;
        private CommandState _commandState;

        // Controllers
        private DirectoryController _directoryController;
        private FileController _fileController;
        private PermissionController _permissionController;
        private UserInterfaceController _userInterfaceController;
        private CommandController _commandController;

        // Start is called before the first frame update
        public void Start()
        {
            _fileSystemState = new FileSystemState();
            _terminalState = new TerminalState();
            _commandState = new CommandState();

            _directoryController = new DirectoryController();
            _fileController = new FileController();
            _permissionController = new PermissionController();
            _userInterfaceController = new UserInterfaceController();
            _commandController = new CommandController();

            InitializeTerminalState(_terminalState);
            InitializeCommandState(_commandState, _terminalState);
            InitializeConsoleText(InputTextObject, addPrompt: true);
            InitializeConsoleText(OutputTextObject, addPrompt: false);
        }

        private void InitializeTerminalState(TerminalState terminalState)
        {
            terminalState.ClearCurrentInput();
            terminalState.ClearPreviousCommands();

            Debug.Assert(InputLengthCharacterLimit > 0, "Input length character limit is invalid.");
            Debug.Assert(InputHistoryLimit > 0, "Input history limit is invalid.");

            var isInputLengthSetSuccess = terminalState.TrySetTerminalInputLengthLimit(InputLengthCharacterLimit);
            var isInputHistorySetSuccess = terminalState.TrySetCommandHistoryLimit(InputHistoryLimit);

            Debug.Assert(isInputLengthSetSuccess, "Failed to set maximum input length.");
            Debug.Assert(isInputHistorySetSuccess, "Failed to set maximum input history limit.");
        }

        private void InitializeConsoleText(Text consoleText, bool addPrompt = false)
        {
            Debug.Assert(consoleText != null, "A console text object is not properly set.");

            // Ensure that the console text is empty to start
            _userInterfaceController.SetUserInterfaceText(consoleText, string.Empty, addPrompt);
        }

        private void InitializeCommandState(CommandState commandState, TerminalState terminalState)
        {
            var helpCommand = new HelpCommand(commandState);
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(helpCommand.GetCommandName(), helpCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `help` command to available command state");

            var clearCommand = new ClearCommand(terminalState);
            isAddCommandSuccess = commandState.TryAddAvailableCommand(clearCommand.GetCommandName(), clearCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add `clear` command to available command state");
        }

        // TODO - Check out https://github.com/sprache/Sprache for text parsing

        // Update is called once per frame
        public void Update()
        {
            // First, figure out if the user has done anything to modify the input
            // TODO - Should also consider the case of up arrow and down arrow to go back to previous inputs (which cannot be handled by Input.inputString)
            var userInteraction = _userInterfaceController.GetUserInteraction(Input.inputString, _terminalState);

            // If the user has modified input, make sure that is reflected back in the UI
            if (userInteraction.IsInputModified)
            {
                _userInterfaceController.SetUserInterfaceText(InputTextObject, userInteraction.ModifiedInput, addPrompt: true);
            }

            // Next, if the user submitted input as part of their interactions, attempt to validate and execute what they submitted
            if (userInteraction.IsInputSubmitted)
            {
                // TODO - Parse the text of the submitted command here
                Debug.Log($"User input submitted: {userInteraction.SubmittedInput}");

                // TODO - This is temporary to work out process flow before text parsing is in place
                string[] args = null;
                if (userInteraction.SubmittedInput.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                {
                    args = new string[] { "help" };
                }

                if (userInteraction.SubmittedInput.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    args = new string[] { "clear" };
                }

                // TODO - Validate the parsed command is a valid command
                // TODO - Continue to validate the command here actually against the list of commands available (syntax and all)
                var userInteractionResponse = new StringBuilder();

                // Check to see that the we can retrieve the command the user wants to execute
                var isCommandRetrievedSuccess = _commandController.TryGetCommand(_commandState, userInteraction.SubmittedInput, out var command);
                if (!isCommandRetrievedSuccess)
                {
                    userInteractionResponse.AppendLine($"Command `{userInteraction.SubmittedInput}` not found.");
                }

                // Execute the command, even if we failed to retrieve the user's command and are using the default command
                var commandResponse = command.ExecuteCommand(args);
                userInteractionResponse.AppendLine(commandResponse);
                userInteraction.IsOutputModified = true;

                // If the command was clear, we do not want to show output for it, otherwise we do want output visible
                var terminalCommand = new TerminalCommand
                {
                    TerminalCommandInput = userInteraction.SubmittedInput,
                    TerminalCommandOutput = userInteractionResponse.ToString(),
                    IsVisibleInTerminal = command.GetType() != typeof(ClearCommand)
                };

                // Add the input to the list of historical inputs if it is a valid input (not empty, null, or over the character limit)
                if (_terminalState.TryValidateInput(userInteraction.SubmittedInput, out var validSubmittedInput))
                {
                    var isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalCommand(terminalCommand);
                    if (!isAddHistoricalInputSuccess && _terminalState.TryRemoveOldestHistoricalCommand())
                    {
                        isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalCommand(terminalCommand);
                    }

                    Debug.Assert(isAddHistoricalInputSuccess, $"Failed to add valid historical input: {validSubmittedInput} with output: {userInteractionResponse}");
                }
            }

            // Finally, if the user's input requires a corresponding change in output, reflect that in the UI
            if (userInteraction.IsOutputModified)
            {
                // If a command was submitted, it has already been added to the previous commands with relevant output
                // We can construct full output to the user with the list of previous commands
                var previousTerminalCommands = _terminalState.GetPreviousTerminalCommands();
                userInteraction.ModifiedOutput = _userInterfaceController.BuildUserInterfaceText(previousTerminalCommands);

                _userInterfaceController.SetUserInterfaceText(OutputTextObject, userInteraction.ModifiedOutput, addPrompt: false);
            }
        }
    }
}