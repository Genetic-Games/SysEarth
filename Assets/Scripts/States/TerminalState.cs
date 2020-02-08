using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.States
{
    public class TerminalState
    {
        [SerializeField]
        private int inputHistoryLimit;

        [SerializeField]
        private int inputLengthLimit;

        // TODO - Constructor needed here for default limit values?  Particularly for testing?

        private string _currentTerminalInput;
        private IList<string> _previousTerminalInputs = new List<string>();

        public void SetInputHistoryLimit(int limitValue)
        {
            inputHistoryLimit = limitValue;
        }

        public void SetInputLengthLimit(int limitValue)
        {
            inputLengthLimit = limitValue;
        }

        public string GetCurrentInput()
        {
            return _currentTerminalInput;
        }

        public void SetCurrentInput(string input)
        {
            if (input.Length > inputLengthLimit)
            {
                _currentTerminalInput = input.Substring(0, inputLengthLimit);
            }

            _currentTerminalInput = input;
        }

        public void AddHistoricalInput(string input)
        {
            _previousTerminalInputs.Add(input);

            if (_previousTerminalInputs.Count > inputHistoryLimit)
            {
                // Remove the oldest inserted element
                _previousTerminalInputs.RemoveAt(0);
            }
        }

        public IList<string> GetPreviousTerminalInputs()
        {
            return _previousTerminalInputs;
        }
    }
}
