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
            InitializeConsoleText(InputTextObject, isInputText: true);
            InitializeConsoleText(OutputTextObject, isInputText: false);
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

        private void InitializeConsoleText(Text consoleText, bool isInputText = false)
        {
            Debug.Assert(consoleText != null, "A console text object is not properly set.");

            // Ensure that the console text is empty to start
            _userInterfaceController.SetUserInterfaceText(consoleText, string.Empty, isInputText);
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
            _userInterfaceController.HandleUserInput(Input.inputString, _terminalState, out var updatedInputText, out var updatedOutputText);

            _userInterfaceController.SetUserInterfaceText(InputTextObject, updatedInputText, isInputText: true);
            _userInterfaceController.SetUserInterfaceText(OutputTextObject, updatedOutputText, isInputText: false);
        }
    }
}