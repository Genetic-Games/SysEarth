using SysEarth.States;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Controllers
{
    public class UserInterfaceController
    {
        private readonly IList<char> _submitCharacters = new List<char> { '\r', '\n' };

        private readonly IList<char> _deleteCharacters = new List<char> { '\b' }; // TODO - This handles backspace, but not delete key

        private const string _newLine = "\n";

        private const string _inputPrompt = "> ";

        // TODO - Should also consider the case of up arrow and down arrow to go back to previous inputs

        public void HandleUserInput(string inputString, TerminalState terminalState, out string updatedInputText, out string updatedOutputText)
        {
            // By default, treat updated input and output text as "null" if the text should not be updated
            updatedInputText = null;
            updatedOutputText = null;

            // Parse which characters the user's have pressed
            foreach (char inputCharacter in inputString)
            {
                // Case of enter or return being pressed to submit the user's input
                if (_submitCharacters.Contains(inputCharacter))
                {
                    HandleSubmitInput(terminalState, out updatedInputText, out updatedOutputText);
                }

                // Case of backspace or delete bring pressed to delete some of user's input not yet submitted
                else if (_deleteCharacters.Contains(inputCharacter))
                {
                    HandleDeleteInput(terminalState, out updatedInputText);
                }

                // Case of every other key, treated as the text of the key pressed
                else
                {
                    HandleTextInput(inputCharacter, terminalState, out updatedInputText);
                }
            }
        }

        public void SetUserInterfaceText(Text textObject, string updatedText, bool isInputText = false)
        {
            // If the updated text is null, that signifies there should be no update for the text to the user
            if (updatedText != null)
            {
                textObject.text = isInputText ? _inputPrompt + updatedText : updatedText;
            }
        }

        private void HandleSubmitInput(TerminalState terminalState, out string updatedInputText, out string updatedOutputText)
        {
            // Get the user's full input (not including enter)
            var userSubmittedInput = terminalState.GetCurrentInput();
            Debug.Log("User input submitted:" + userSubmittedInput);

            // TODO - Do something with the input here
            // TODO - Parse the input and then see if it matches any existing commands in the command state, validate, then execute that command
            // TODO - Might actually be better just to store / return the input somewhere, saying it's ready for parsing elsewhere

            // Clear the user's input since it has been submitted
            terminalState.ClearCurrentInput();
            updatedInputText = string.Empty;

            // Add the input to the list of historical inputs, removing old inputs as necessary
            var isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(userSubmittedInput);
            if (!isAddHistoricalInputSuccess && terminalState.TryRemoveOldestHistoricalInput())
            {
                isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(userSubmittedInput);
            }

            Debug.Assert(isAddHistoricalInputSuccess, "Failed to add historical input: " + userSubmittedInput);

            // Show the previous terminal inputs to the user
            // TODO - Make this so that it shows previous terminal inputs and outputs associated
            var previousTerminalInputs = terminalState.GetPreviousTerminalInputs();
            updatedOutputText = string.Join(_newLine, previousTerminalInputs);
        }

        private void HandleDeleteInput(TerminalState terminalState, out string updatedInputText)
        {
            // By default, treat updated input text as "null" if the text should not be updated
            updatedInputText = null;

            var currentInput = terminalState.GetCurrentInput();

            // If the last character is being deleted, the state should be cleared differently
            if (currentInput.Length == 1)
            {
                terminalState.ClearCurrentInput();
                updatedInputText = string.Empty;
            }

            // Otherwise if there is more than one character, delete the last character
            else if (currentInput.Length != 0)
            {
                // If we have characters to delete, remove them and reflect that change back to the user
                var updatedInput = currentInput.Remove(currentInput.Length - 1);
                var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

                if (isSetInputSuccess)
                {
                    updatedInputText = updatedInput;
                }

                Debug.Assert(isSetInputSuccess, "Failed to set current input: " + updatedInput);
            }
        }

        private void HandleTextInput(char inputCharacter, TerminalState terminalState, out string updatedInputText)
        {
            // By default, treat updated input text as "null" if the text should not be updated
            updatedInputText = null;

            var currentInput = terminalState.GetCurrentInput();
            var updatedInput = currentInput + inputCharacter;
            var isSetInputSuccess = terminalState.TrySetCurrentInput(updatedInput);

            if (isSetInputSuccess)
            {
                updatedInputText = updatedInput;
            }
            else
            { 
                // We may be over the character limit, so check if the input is valid before asserting that it should have been set
                var isValidInput = terminalState.TryValidateInput(updatedInput, out var validInput);
                Debug.Assert(!isSetInputSuccess && !isValidInput, "Failed to set current input with valid input: " + validInput);
            }
        }
    }
}
