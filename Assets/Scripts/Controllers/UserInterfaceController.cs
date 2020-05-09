using SysEarth.Models;
using SysEarth.States;
using System.Collections.Generic;
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
                    userInteraction.IsInputSubmitted = true;

                    // Get the user's full input (not including enter or any prompt characters)
                    userInteraction.SubmittedInput = terminalState.GetCurrentInput();
                    Debug.Log($"User input submitted: {userInteraction.SubmittedInput}");

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

        // TODO - Leaving the below for reference, but only the output cases and execute cases are relevant now
        // TODO - Convert below to properly handle output text cases and execute command cases where applicable

        //private void HandleSubmitInput(TerminalState terminalState, out string updatedInputText, out string updatedOutputText)
        //{
        //    // Get the user's full input (not including enter)
        //    var userSubmittedInput = terminalState.GetCurrentInput();
        //    Debug.Log($"User input submitted: {userSubmittedInput}");

        //    // Clear the user's input since it has been submitted, regardless of its validity
        //    // TODO - Move this elsewhere, it's breaking the single responsibility principle
        //    terminalState.ClearCurrentInput();
        //    updatedInputText = string.Empty;

        //    // Add the input to the list of historical inputs if it is a valid input (not empty, null, or over the character limit)
        //    // At this point, we know if the input is valid from the terminal perspective, but not if it maps to a valid command with valid parameters
        //    if (terminalState.TryValidateInput(userSubmittedInput, out var validSubmittedInput))
        //    {
        //        var isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(validSubmittedInput);
        //        if (!isAddHistoricalInputSuccess && terminalState.TryRemoveOldestHistoricalInput())
        //        {
        //            isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(validSubmittedInput);
        //        }

        //        Debug.Assert(isAddHistoricalInputSuccess, $"Failed to add valid historical input: {validSubmittedInput}");
        //    }

        //    // Show the previous terminal inputs to the user
        //    // TODO - Make this so that it shows previous terminal inputs and outputs associated
        //    // TODO - Might make sense to do this action elsewhere, as a different part of the functionality in UserInterfaceController
        //    var previousTerminalInputs = terminalState.GetPreviousTerminalInputs();
        //    updatedOutputText = string.Join(_newLine, previousTerminalInputs);
        //}

        //private void HandleDeleteInput(TerminalState terminalState, out string updatedInputText)
        //{
        //    // By default, treat updated input text as "null" if the text should not be updated
        //    updatedInputText = null;

        //    var currentInput = terminalState.GetCurrentInput();

        //    // If the last character is being deleted, the state should be cleared differently
        //    if (currentInput.Length == 1)
        //    {
        //        terminalState.ClearCurrentInput();
        //        updatedInputText = string.Empty;
        //    }

        //    // Otherwise if there is more than one character, delete the last character
        //    else if (currentInput.Length != 0)
        //    {
        //        // If we have characters to delete, remove them and reflect that change back to the user
        //        var updatedInput = currentInput.Remove(currentInput.Length - 1);
        //        var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

        //        if (isSetInputSuccess)
        //        {
        //            updatedInputText = updatedInput;
        //        }

        //        Debug.Assert(isSetInputSuccess, $"Failed to set current input: {updatedInput}");
        //    }
        //}

        //private void HandleTextInput(char inputCharacter, TerminalState terminalState, out string updatedInputText)
        //{
        //    // By default, treat updated input text as "null" if the text should not be updated
        //    updatedInputText = null;

        //    var currentInput = terminalState.GetCurrentInput();
        //    var updatedInput = currentInput + inputCharacter;
        //    var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

        //    if (isSetInputSuccess)
        //    {
        //        updatedInputText = updatedInput;
        //    }
        //    else
        //    { 
        //        // We may be over the character limit, so check if the input is valid before asserting that it should have been set
        //        var isValidInput = terminalState.TryValidateInput(updatedInput, out var validInput);
        //        Debug.Assert(!isSetInputSuccess && !isValidInput, $"Failed to set current input with valid input: {validInput}");
        //    }
        //}
    }
}
