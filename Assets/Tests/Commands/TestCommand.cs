using SysEarth.Commands;
using System;
using System.Collections.Generic;

namespace SysEarth.Tests.Commands
{
    /// <summary>
    /// This command class is only meant to be used for testing, not for player usage
    /// </summary>
    public class TestCommand : ICommand
    {
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

        public bool TryValidateArguments(out string responseMessage, params string[] args)
        {
            throw new NotImplementedException();
        }

        public string ExecuteCommand(params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
