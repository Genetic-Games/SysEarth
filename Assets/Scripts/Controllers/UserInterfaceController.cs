using SysEarth.Models;
using SysEarth.States;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class UserInterfaceController
    {
        private readonly IList<char> _submitCharacters = new List<char> { '\r', '\n' };
        private readonly IList<char> _deleteCharacters = new List<char> { '\b' }; // Note - only backspace is supported by Input, delete is unsupported

        private const string _newLine = "\n";
        private const string _userInputPrompt = "|> ";

        // TODO - Should also consider the case of up arrow and down arrow to go back to previous inputs

        public UserInteraction GetUserInteraction(string userInputString, TerminalState terminalState)
        {
            var userInteraction = new UserInteraction();

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

        public void SetUserInterfaceText(Text textObject, string updatedText, bool addPrompt = false)
        {
            // If the updated text is null, that signifies there should be no update for the text to the user
            if (updatedText != null)
            {
                textObject.text = addPrompt ? _userInputPrompt + updatedText : updatedText;
            }
        }

        public string BuildUserInterfaceText(IList<TerminalCommand> terminalCommands)
        {
            var userInterfaceText = new StringBuilder();

            foreach (var terminalCommand in terminalCommands)
            {
                if (terminalCommand.IsVisibleInTerminal)
                {
                    userInterfaceText.AppendLine(_userInputPrompt + terminalCommand.TerminalCommandInput);
                    userInterfaceText.AppendLine(terminalCommand.TerminalCommandOutput);
                    userInterfaceText.AppendLine(); // Empty line for better readability between each pair of input and output
                }
            }

            return userInterfaceText.ToString();
        }
    }
}
