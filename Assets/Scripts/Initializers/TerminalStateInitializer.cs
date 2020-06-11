using SysEarth.States;
using UnityEngine;

namespace SysEarth.Initializers
{
    public class TerminalStateInitializer
    {
        public void InitializeTerminalState(TerminalState terminalState, int inputLengthCharacterLimit, int inputHistoryLimit)
        {
            Debug.Assert(inputLengthCharacterLimit > 0, "Input length character limit is invalid.");
            Debug.Assert(inputHistoryLimit > 0, "Input history limit is invalid.");

            var isInputLengthSetSuccess = terminalState.TrySetTerminalInputLengthLimit(inputLengthCharacterLimit);
            var isInputHistorySetSuccess = terminalState.TrySetCommandHistoryLimit(inputHistoryLimit);

            Debug.Assert(isInputLengthSetSuccess, "Failed to set maximum input length.");
            Debug.Assert(isInputHistorySetSuccess, "Failed to set maximum input history limit.");
        }

        public void ClearTerminalState(TerminalState terminalState)
        {
            terminalState.ClearCurrentInput();
            terminalState.ClearPreviousCommands();
        }
    }
}
