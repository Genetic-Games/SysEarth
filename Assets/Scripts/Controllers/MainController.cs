using SysEarth.Commands;
using SysEarth.Models;
using SysEarth.States;
using System;
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
            InitializeCommandState(_commandState);
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

        private void InitializeCommandState(CommandState commandState)
        {
            var helpCommand = new HelpCommand(commandState);
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(helpCommand.GetCommandName(), helpCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add help command to available command state");
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
                if (userInteraction.SubmittedInput.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                {
                    var args = new string[] { "help" };
                    var command = _commandController.GetCommand(_commandState, userInteraction.SubmittedInput);
                    var userInteractionResponse = command.ExecuteCommand(args);

                    userInteraction.IsOutputModified = true;

                    var terminalCommand = new TerminalCommand
                    {
                        TerminalCommandInput = userInteraction.SubmittedInput,
                        TerminalCommandOutput = userInteractionResponse
                    };

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

                if (userInteraction.SubmittedInput.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    userInteraction.IsOutputModified = true;
                    _terminalState.HidePreviousCommands();
                }

                // TODO - Validate the parsed command is a valid command

                // Add the input to the list of historical inputs if it is a valid input (not empty, null, or over the character limit)
                // At this point, we know if the input is valid from the terminal perspective, but not if it maps to a valid command with valid parameters
                //if (_terminalState.TryValidateInput(userInteraction.SubmittedInput, out var validSubmittedInput))
                //{
                //    var isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalCommand(validSubmittedInput);
                //    if (!isAddHistoricalInputSuccess && _terminalState.TryRemoveOldestHistoricalInput())
                //    {
                //        isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalCommand(validSubmittedInput);
                //    }

                //    Debug.Assert(isAddHistoricalInputSuccess, $"Failed to add valid historical input: {validSubmittedInput}");
                //}

                // TODO - Continue to validate the command here actually against the list of commands available (syntax and all)
                // TODO - Execute the command submitted here
                // TODO - Indicate that the output is modified (if it is) and set the modified output here
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