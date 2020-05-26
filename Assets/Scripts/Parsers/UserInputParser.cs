using SysEarth.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SysEarth.Parsers
{
    public class UserInputParser
    {
        private readonly IList<char> _delimiters = new List<char> { ' ', '\t', '\n', '\r' };

        public bool TryParseUserInput(string rawInput, out ParsedUserInput parsedInput)
        {
            parsedInput = new ParsedUserInput();

            if (rawInput == null || !rawInput.Any())
            {
                return false;
            }

            var splitInput = rawInput.Split(_delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries)?.ToList();
            if (splitInput == null || !splitInput.Any())
            {
                return false;
            }
            else
            {
                parsedInput.Arguments = splitInput;
            }

            var commandName = splitInput.FirstOrDefault();
            if (string.IsNullOrEmpty(commandName))
            {
                return false;
            }
            else
            {
                parsedInput.CommandName = commandName;
            }

            return true;
        }
    }
}
