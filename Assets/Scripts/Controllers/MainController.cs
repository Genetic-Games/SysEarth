using SysEarth.Commands;
using SysEarth.States;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class MainController : MonoBehaviour
    {
        // Scene References
        public Text InputText;
        public Text OutputText;

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

        // Start is called before the first frame update
        public void Start()
        {
            _fileSystemState = new FileSystemState();
            _terminalState = new TerminalState();
            _commandState = new CommandState();

            InitializeTerminalState(_terminalState);

            _directoryController = new DirectoryController();
            _fileController = new FileController();
            _permissionController = new PermissionController();

            AddInitialCommands(_commandState);

            InitializeConsoleText(InputText); // TODO - Make this start with "> " for a visual prompt to the user
            InitializeConsoleText(OutputText);
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

        private void InitializeConsoleText(Text consoleText)
        {
            Debug.Assert(consoleText != null, "A console text object is not properly set.");

            // Ensure that the console text is empty to start
            consoleText.text = string.Empty;
        }

        private void AddInitialCommands(CommandState commandState)
        {
            var helpCommand = new HelpCommand();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(helpCommand.GetCommandName(), helpCommand);

            Debug.Assert(isAddCommandSuccess, "Failed to add help command to available command state");
        }

        // TODO - Check out https://github.com/sprache/Sprache for text parsing

        // Update is called once per frame
        public void Update()
        {
            // TODO - Move this to an input controller
            foreach (char inputCharacter in Input.inputString)
            {
                if (inputCharacter == '\n' || inputCharacter == '\r') // Has enter or return been pressed?
                {
                    var userSubmittedInput = _terminalState.GetCurrentInput();
                    Debug.Log("User input submitted:" + userSubmittedInput);

                    // TODO - Do something with the input here
                    // TODO - Parse the input and then see if it matches any existing commands in the command state, validate, then execute that command

                    _terminalState.ClearCurrentInput();
                    InputText.text = string.Empty;

                    var isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalInput(userSubmittedInput);
                    if (!isAddHistoricalInputSuccess && _terminalState.TryRemoveOldestHistoricalInput())
                    {
                        isAddHistoricalInputSuccess = _terminalState.TryAddHistoricalInput(userSubmittedInput);
                    }

                    var previousTerminalInputs = _terminalState.GetPreviousTerminalInputs();
                    OutputText.text = string.Join("\n", previousTerminalInputs);
                }
                else if (inputCharacter == '\b') // Has backspace or delete been pressed?
                {
                    var currentInput = _terminalState.GetCurrentInput();
                    if (currentInput.Length != 0)
                    {
                        var updatedInput = currentInput.Substring(0, currentInput.Length - 1);
                        var isDeleteCharacterSuccess = _terminalState.TrySetCurrentInput(updatedInput);

                        if (isDeleteCharacterSuccess)
                        {
                            InputText.text = updatedInput;
                        }
                    }
                }
                else
                {
                    var currentInput = _terminalState.GetCurrentInput();
                    var updatedInput = currentInput + inputCharacter;
                    var isSetInputSuccess = _terminalState.TrySetCurrentInput(updatedInput);

                    if (isSetInputSuccess)
                    {
                        InputText.text = updatedInput;
                    }
                }
            }
        }
    }
}