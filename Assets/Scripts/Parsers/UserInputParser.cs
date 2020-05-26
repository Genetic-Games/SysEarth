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

            // Cannot parse input that does not exist
            if (rawInput == null || !rawInput.Any())
            {
                return false;
            }

            // Split the input by white space delimiters to distinguish between the command and its arguments
            var splitInput = rawInput.Split(_delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries)?.ToList();
            if (splitInput == null || !splitInput.Any())
            {
                return false;
            }

            // Store the arguments, including the command name, to be used later when executing the command
            parsedInput.Arguments = splitInput;

            // Extract just the command name to use to pick the command class to execute
            var commandName = splitInput.FirstOrDefault();
            if (string.IsNullOrEmpty(commandName))
            {
                parsedInput = new ParsedUserInput();
                return false;
            }

            parsedInput.CommandName = commandName;
            return true;
        }
    }
}
