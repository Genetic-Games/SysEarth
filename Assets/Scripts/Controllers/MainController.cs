using SysEarth.Commands;
using SysEarth.States;
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

            InitializeTerminalState(_terminalState);
            InitializeCommandState(_commandState);
            InitializeConsoleText(InputTextObject, addPrompt: true);
            InitializeConsoleText(OutputTextObject, addPrompt: false);
        }

        private void InitializeTerminalState(TerminalState terminalState)
        {
            terminalState.ClearCurrentInput();
            terminalState.ClearPreviousInputs();

            Debug.Assert(InputLengthCharacterLimit > 0, "Input length character limit is invalid.");
            Debug.Assert(InputHistoryLimit > 0, "Input history limit is invalid.");

            var isInputLengthSetSuccess = terminalState.TrySetInputLengthLimit(InputLengthCharacterLimit);
            var isInputHistorySetSuccess = terminalState.TrySetInputHistoryLimit(InputHistoryLimit);

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
            var helpCommand = new HelpCommand();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(helpCommand.GetCommandName(), helpCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add help command to available command state");
        }

        // TODO - Check out https://github.com/sprache/Sprache for text parsing

        // Update is called once per frame
        public void Update()
        {
            var userInteraction = _userInterfaceController.GetUserInteraction(Input.inputString, _terminalState);

            if (userInteraction.IsInputModified)
            {
                _userInterfaceController.SetUserInterfaceText(InputTextObject, userInteraction.ModifiedInput, addPrompt: true);
            }

            if (userInteraction.IsInputSubmitted)
            {
                // TODO - Parse the text of the submitted command here
                Debug.Log($"User input submitted: {userInteraction.SubmittedInput}");

                // TODO - Validate the parsed command is a valid command

                // Add the input to the list of historical inputs if it is a valid input (not empty, null, or over the character limit)
                // At this point, we know if the input is valid from the terminal perspective, but not if it maps to a valid command with valid parameters
                if (_terminalState.TryValidateInput(userInteraction.SubmittedInput, out var validSubmittedInput))
                {
                    var isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalInput(validSubmittedInput);
                    if (!isAddHistoricalInputSuccess && _terminalState.TryRemoveOldestHistoricalInput())
                    {
                        isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalInput(validSubmittedInput);
                    }

                    Debug.Assert(isAddHistoricalInputSuccess, $"Failed to add valid historical input: {validSubmittedInput}");
                }

                // TODO - Continue to validate the command here actually against the list of commands available (syntax and all)
                // TODO - Execute the command submitted here
                // TODO - Indicate that the output is modified (if it is) and set the modified output here
            }

            if (userInteraction.IsOutputModified)
            {
                _userInterfaceController.SetUserInterfaceText(OutputTextObject, userInteraction.ModifiedOutput, addPrompt: false);
            }
        }
    }
}