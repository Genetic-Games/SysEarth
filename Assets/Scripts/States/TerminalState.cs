using SysEarth.Models;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.States
{
    public class TerminalState
    {
        private int _terminalCommandHistoryLimit;
        private int _terminalInputLengthLimit;
        private string _currentTerminalInput;
        private IList<TerminalCommand> _previousTerminalCommands;

        public TerminalState()
        {
            _previousTerminalCommands = new List<TerminalCommand>();
        }

        public int GetCommandHistoryLimit()
        {
            return _terminalCommandHistoryLimit;
        }

        public int GetInputLengthLimit()
        {
            return _terminalInputLengthLimit;
        }

        public string GetCurrentInput()
        {
            return _currentTerminalInput;
        }

        public IList<TerminalCommand> GetPreviousTerminalCommands()
        {
            return _previousTerminalCommands;
        }

        public bool TrySetCommandHistoryLimit(int limitValue)
        {
            if (limitValue <= 0)
            {
                return false;
            }

            _terminalCommandHistoryLimit = limitValue;
            return true;
        }

        public bool TrySetTerminalInputLengthLimit(int limitValue)
        {
            if (limitValue <= 0)
            {
                return false;
            }

            _terminalInputLengthLimit = limitValue;
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
            _currentTerminalInput = string.Empty;
        }

        public void ClearPreviousCommands()
        {
            _previousTerminalCommands.Clear();
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

        public bool TryAddHistoricalCommand(TerminalCommand historicalCommand)
        {
            if (historicalCommand == null || 
                string.IsNullOrEmpty(historicalCommand.TerminalCommandInput) || 
                string.IsNullOrEmpty(historicalCommand.TerminalCommandOutput) || 
                _previousTerminalCommands.Count == GetCommandHistoryLimit())
            {
                return false;
            }

            _previousTerminalCommands.Add(historicalCommand);
            return true;
        }

        public bool TryRemoveOldestHistoricalCommand()
        {
            // Only remove if we are at the limit
            if (_previousTerminalCommands.Count < GetCommandHistoryLimit())
            {
                return false;
            }

            // Remove the oldest inserted element
            var oldestCommand = _previousTerminalCommands.FirstOrDefault();
            _previousTerminalCommands.Remove(oldestCommand);
            return true;
        }
    }
}
