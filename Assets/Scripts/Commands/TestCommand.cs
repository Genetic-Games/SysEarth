using System;
using System.Collections.Generic;

namespace SysEarth.Commands
{
    /// <summary>
    /// This command class is only meant to be used for testing, not for player usage
    /// </summary>
    public class TestCommand : ICommand
    {
        public void ExecuteCommand(IList<string> args)
        {
            throw new NotImplementedException();
        }

        public string GetCommandDescription()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetCommandFlagDescriptions()
        {
            throw new NotImplementedException();
        }

        public string GetCommandName()
        {
            throw new NotImplementedException();
        }

        public IList<string> GetExampleUsages()
        {
            throw new NotImplementedException();
        }

        public IList<string> GetResponseMessages()
        {
            throw new NotImplementedException();
        }

        public bool ValidateArguments(IList<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
