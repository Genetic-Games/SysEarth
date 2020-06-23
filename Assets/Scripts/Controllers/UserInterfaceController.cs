using SysEarth.Models;
using SysEarth.States;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class UserInterfaceController
    {
        private readonly IList<char> _submitCharacters = new List<char> { '\r', '\n' };
        private readonly IList<char> _deleteCharacters = new List<char> { '\b' }; // Note - only backspace is supported by Input, delete is unsupported

        private const string _userInputPrompt = "|>";

        // TODO - Consider the case where a "cursor" is implemented, allowing deletion / modification at various places in the string and having to keep track of placement

        public UserInteraction GetUserInteraction(string userInputString, bool isUpArrowPressed, bool isDownArrowPressed, TerminalState terminalState)
        {
            // Make sure we actually have a user interaction to deal with this frame - if not, short circuit and just return
            var userInteraction = new UserInteraction();
            if (string.IsNullOrEmpty(userInputString) && !isUpArrowPressed && !isDownArrowPressed)
            {
                return userInteraction;
            }

            // First, check to see if the user hit the up arrow key to scroll through their last inputs
            if (isUpArrowPressed)
            {
                var previousCommands = terminalState.GetPreviousTerminalCommands();
                var currentSelectedCommandNumber = terminalState.GetTerminalCommandSelectedNumber();

                var selectedCommand = GetPreviousCommandWithArrowKeys(previousCommands, currentSelectedCommandNumber, isUpArrow: true);

                // If, for whatever reason, we could not find the selected command for the user, do not try to change their input or do anything
                if (selectedCommand == null)
                {
                    return userInteraction;
                }

                // Clear out and modify the user's current input, they want this replaced with a previous submitted input
                terminalState.ClearCurrentInput();
                var isSetInputSuccess = terminalState.TrySetCurrentInput(selectedCommand.TerminalCommandInput);

                if (isSetInputSuccess)
                {
                    userInteraction.IsInputModified = true;
                    userInteraction.ModifiedInput = selectedCommand.TerminalCommandInput;
                    terminalState.SetTerminalCommandSelectedNumber(selectedCommand.TerminalCommandNumber);
                }
                else
                {
                    Debug.Assert(isSetInputSuccess, $"Failed to set current input: {selectedCommand.TerminalCommandInput}");
                }

                return userInteraction;
            }

            // Next, check to see if the user hit the down arrow key to scroll through their last inputs
            if (isDownArrowPressed)
            {
                var previousCommands = terminalState.GetPreviousTerminalCommands();
                var currentSelectedCommandNumber = terminalState.GetTerminalCommandSelectedNumber();

                var selectedCommand = GetPreviousCommandWithArrowKeys(previousCommands, currentSelectedCommandNumber, isUpArrow: false);

                // If, for whatever reason, we could not find the selected command for the user, do not try to change their input or do anything
                if (selectedCommand == null)
                {
                    return userInteraction;
                }

                // Clear out and modify the user's current input, they want this replaced with a previous submitted input
                terminalState.ClearCurrentInput();
                var isSetInputSuccess = terminalState.TrySetCurrentInput(selectedCommand.TerminalCommandInput);

                if (isSetInputSuccess)
                {
                    userInteraction.IsInputModified = true;
                    userInteraction.ModifiedInput = selectedCommand.TerminalCommandInput;
                    terminalState.SetTerminalCommandSelectedNumber(selectedCommand.TerminalCommandNumber);
                }
                else
                {
                    Debug.Assert(isSetInputSuccess, $"Failed to set current input: {selectedCommand.TerminalCommandInput}");
                }

                return userInteraction;
            }

            // If the user starts typing, we know they did not try to find a previous command (or are modifying a previous one)
            // At that point, remove any command that may have been selected and treat this as a brand new input case
            terminalState.ClearTerminalCommandSelectedNumber();

            // Parse which characters the user's have pressed
            foreach (char userInputCharacter in userInputString)
            {
                // Case of enter or return being pressed to submit the user's input
                if (_submitCharacters.Contains(userInputCharacter))
                {
                    // Get the user's full input (not including enter or any prompt characters)
                    userInteraction.IsInputSubmitted = true;
                    userInteraction.SubmittedInput = terminalState.GetCurrentInput();

                    // Clear the user's input since it has been submitted, regardless of its validity
                    terminalState.ClearCurrentInput();
                    userInteraction.IsInputModified = true;
                    userInteraction.ModifiedInput = string.Empty;
                }

                // Case of backspace or delete bring pressed to delete some of user's input not yet submitted
                else if (_deleteCharacters.Contains(userInputCharacter))
                {
                    var currentInput = terminalState.GetCurrentInput();

                    // If the last character is being deleted, the state should be cleared differently
                    if (currentInput.Length == 1)
                    {
                        terminalState.ClearCurrentInput();
                        userInteraction.IsInputModified = true;
                        userInteraction.ModifiedInput = string.Empty;
                    }

                    // Otherwise if there is more than one character, delete the last character
                    else if (currentInput.Length != 0)
                    {
                        // If we have characters to delete, remove them and reflect that change back to the user
                        var updatedInput = currentInput.Remove(currentInput.Length - 1);
                        var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

                        if (isSetInputSuccess)
                        {
                            userInteraction.IsInputModified = true;
                            userInteraction.ModifiedInput = updatedInput;
                        }

                        Debug.Assert(isSetInputSuccess, $"Failed to set current input: {updatedInput}");
                    }
                }

                // Case of every other key, treated as the text of the key pressed
                else
                {
                    var currentInput = terminalState.GetCurrentInput();
                    var updatedInput = currentInput + userInputCharacter;
                    var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

                    if (isSetInputSuccess)
                    {
                        userInteraction.IsInputModified = true;
                        userInteraction.ModifiedInput = updatedInput;
                    }
                    else
                    {
                        // We may be over the character limit, so check if the input is valid before asserting that it should have been set
                        var isValidInput = terminalState.TryValidateInput(updatedInput, out var validInput);
                        Debug.Assert(!isSetInputSuccess && !isValidInput, $"Failed to set current input with valid input: {validInput}");
                    }
                }
            }

            return userInteraction;
        }

        private TerminalCommand GetPreviousCommandWithArrowKeys(
            IList<TerminalCommand> previousCommands, 
            ulong? currentSelectedCommandNumber,
            bool isUpArrow = true)
        {
            // This is the first time the user is scrolling through submitted entries to find a previous one (cannot start with down)
            if (currentSelectedCommandNumber == null && isUpArrow)
            {
                return previousCommands.OrderByDescending(command => command.TerminalCommandNumber).FirstOrDefault();
            }

            // Check to see if we are already in the process of scrolling through previous commands (one has already been selected)
            else if (currentSelectedCommandNumber != null && isUpArrow)
            {
                // If user hit the up arrow, that means that they want the previous command submitted before the one they are currently on
                // Counter-intuitive, but that means they want a lower command number (it was entered earlier, so has a lower number)
                return previousCommands
                    .Where(command => command.TerminalCommandNumber < currentSelectedCommandNumber)
                    .OrderByDescending(command => command.TerminalCommandNumber)
                    .FirstOrDefault();
            }

            else if (currentSelectedCommandNumber != null && !isUpArrow)
            {
                // If user hit the down arrow, that means they want the next command submitted after the one they are currently on
                // Counter-intuitive, but that means they want a higher command number (it was entered later, so has a higher number)
                return previousCommands
                    .Where(command => command.TerminalCommandNumber > currentSelectedCommandNumber)
                    .OrderBy(command => command.TerminalCommandNumber)
                    .FirstOrDefault();
            }

            return null;
        }

        // TODO - Add username here before path, similar to Bash
        public void SetUserInterfaceTextWithInputPrompt(Text textObject, string updatedText, string currentDirectoryPath = null)
        {
            // If the updated text is null, that signifies there should be no update for the text to the user
            if (updatedText != null)
            {
                textObject.text = $"{currentDirectoryPath ?? string.Empty} {_userInputPrompt} {updatedText}";
            }
        }

        public void SetUserInterfaceText(Text textObject, string updatedText)
        {
            // If the updated text is null, that signifies there should be no update for the text to the user
            if (updatedText != null)
            {
                textObject.text = updatedText;
            }
        }

        public string BuildUserInterfaceText(IList<TerminalCommand> terminalCommands)
        {
            var userInterfaceText = new StringBuilder();

            foreach (var terminalCommand in terminalCommands)
            {
                if (terminalCommand.IsVisibleInTerminal)
                {
                    userInterfaceText.AppendLine($"{terminalCommand.TerminalCommandPath} {_userInputPrompt} {terminalCommand.TerminalCommandInput}");
                    userInterfaceText.AppendLine(terminalCommand.TerminalCommandOutput);
                    userInterfaceText.AppendLine(); // Empty line for better readability between each pair of input and output
                }
            }

            return userInterfaceText.ToString();
        }
    }
}
