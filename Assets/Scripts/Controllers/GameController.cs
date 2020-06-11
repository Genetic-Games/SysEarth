using SysEarth.Commands;
using SysEarth.Initializers;
using SysEarth.Models;
using SysEarth.Parsers;
using SysEarth.States;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class GameController : MonoBehaviour
    {
        // Constants
        private const string _helpCommandName = "help";

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

        // Parsers
        private UserInputParser _userInputParser;

        // Initializers
        private TerminalStateInitializer _terminalStateInitializer;
        private FileSystemStateInitializer _fileSystemStateInitializer;
        private CommandStateInitializer _commandStateInitializer;
        private UserInterfaceInitializer _userInterfaceInitializer;

        // Initialization
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

            _terminalStateInitializer = new TerminalStateInitializer();
            _fileSystemStateInitializer = new FileSystemStateInitializer();
            _commandStateInitializer = new CommandStateInitializer();
            _userInterfaceInitializer = new UserInterfaceInitializer();

            _userInputParser = new UserInputParser();

            // Initialize the state of each system piece for game start
            _terminalStateInitializer.InitializeTerminalState(_terminalState, InputLengthCharacterLimit, InputHistoryLimit);
            _terminalStateInitializer.ClearTerminalState(_terminalState);

            var commandsInitialized = _commandStateInitializer.InitializeCommandState(_commandState, _terminalState, _fileSystemState, _directoryController);
            _fileSystemStateInitializer.InitializeFileSystemState(_fileSystemState, _permissionController, _directoryController);
            _fileSystemStateInitializer.InitializeCommandsInFileSystemState(_fileSystemState, _permissionController, _directoryController, _fileController, commandsInitialized);

            _userInterfaceInitializer.InitializeConsoleText(_userInterfaceController, InputTextObject, addPrompt: true);
            _userInterfaceInitializer.InitializeConsoleText(_userInterfaceController, OutputTextObject, addPrompt: false);
        }

        // Game Loop - Executed Once Per Frame
        public void Update()
        {
            // First, figure out if the user has done anything to modify the input
            // TODO - Should also consider the case of up arrow and down arrow to go back to previous inputs (which cannot be handled by Input.inputString)
            var userInteraction = _userInterfaceController.GetUserInteraction(Input.inputString, _terminalState);

            // If the user has modified input, make sure that is reflected back in the UI
            if (userInteraction.IsInputModified)
            {
                // TODO - Figure out how to show the user the current directory too as part of the input (before the prompt), like Bash does
                _userInterfaceController.SetUserInterfaceText(InputTextObject, userInteraction.ModifiedInput, addPrompt: true);
            }

            // Next, if the user submitted input as part of their interactions, attempt to validate and execute what they submitted
            if (userInteraction.IsInputSubmitted)
            {
                var userInteractionResponse = new StringBuilder();

                // Since the user submitted input, we now need to parse that input
                Debug.Log($"User input submitted: `{userInteraction.SubmittedInput}`");

                var isParseInputSuccess = _userInputParser.TryParseUserInput(userInteraction.SubmittedInput, out var parsedUserSubmittedInput);
                if (!isParseInputSuccess)
                {
                    Debug.Log($"Failed to parse user input: `{userInteraction.SubmittedInput}`");
                }

                // Extract the arguments into a parameterized array
                var args = parsedUserSubmittedInput.Arguments?.ToArray();

                // Check to see that the we can retrieve the command the user wants to execute from the parsed input
                var isCommandRetrievedSuccess = _commandController.TryGetCommand(_commandState, parsedUserSubmittedInput.CommandName, out var command);
                if (!isCommandRetrievedSuccess)
                {
                    userInteractionResponse.AppendLine($"Command `{parsedUserSubmittedInput.CommandName}` not found.");
                    userInteractionResponse.AppendLine($"Run `{_helpCommandName}` for a list of available commands");
                }

                // Execute the command if we successfully retrieved it
                // Note - Each command is in charge of its own validation and if / how it executes after succeeding or failing validation
                else
                {
                    var commandResponse = command.ExecuteCommand(args);
                    userInteractionResponse.AppendLine(commandResponse);
                }

                // Mark that the user's output will change based on this latest terminal command
                userInteraction.IsOutputModified = true;
                var terminalCommand = new TerminalCommand
                {
                    TerminalCommandInput = userInteraction.SubmittedInput,
                    TerminalCommandOutput = userInteractionResponse.ToString(),

                    // If the command was a valid `clear` command, we do not want to show output for it, otherwise we do want output visible
                    IsVisibleInTerminal = command == null || command.GetType() != typeof(ClearCommand) || !command.TryValidateArguments(out _, args)
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