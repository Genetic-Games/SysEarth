using System.Collections.Generic;
using System.Linq;

namespace SysEarth.States
{
    public class TerminalState
    {
        private int _inputHistoryLimit;
        private int _inputLengthLimit;
        private string _currentTerminalInput;
        private IList<string> _previousTerminalInputs;

        public TerminalState()
        {
            _previousTerminalInputs = new List<string>();
        }

        public int GetInputHistoryLimit()
        {
            return _inputHistoryLimit;
        }

        public int GetInputLengthLimit()
        {
            return _inputLengthLimit;
        }

        public string GetCurrentInput()
        {
            return _currentTerminalInput;
        }

        public IList<string> GetPreviousTerminalInputs()
        {
            return _previousTerminalInputs;
        }

        public bool TrySetInputHistoryLimit(int limitValue)
        {
            if (limitValue <= 0)
            {
                return false;
            }

            _inputHistoryLimit = limitValue;
            return true;
        }

        public bool TrySetInputLengthLimit(int limitValue)
        {
            if (limitValue <= 0)
            {
                return false;
            }

            _inputLengthLimit = limitValue;
            return true;
        }

        public bool TrySetCurrentInput(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length > GetInputLengthLimit())
            {
                return false;
            }

            _currentTerminalInput = input;
            return true;
        }

        public void ClearCurrentInput()
        {
            _currentTerminalInput = null;
        }

        public void ClearPreviousInputs()
        {
            _previousTerminalInputs.Clear();
        }

        public bool TryValidateInput(string input, out string validInput)
        {
            validInput = null;
            if (string.IsNullOrEmpty(input) || input.Length > GetInputLengthLimit())
            {
                return false;
            }

            validInput = input;
            return true;
        }

        public bool TryTruncateInput(string input, out string truncatedInput)
        {
            truncatedInput = null;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (input.Length > GetInputLengthLimit())
            {
                truncatedInput = input.Substring(0, GetInputLengthLimit());
                return true;
            }

            return false;
        }

        public bool TryAddHistoricalInput(string input)
        {
            if (string.IsNullOrEmpty(input) || _previousTerminalInputs.Count == GetInputHistoryLimit())
            {
                return false;
            }

            _previousTerminalInputs.Add(input);
            return true;
        }

        public bool TryRemoveOldestHistoricalInput()
        {
            // Only remove if we are at the limit
            if (_previousTerminalInputs.Count < GetInputHistoryLimit())
            {
                return false;
            }

            // Remove the oldest inserted element
            var oldestInput = _previousTerminalInputs.FirstOrDefault();
            _previousTerminalInputs.Remove(oldestInput);
            return true;
        }
    }
}
