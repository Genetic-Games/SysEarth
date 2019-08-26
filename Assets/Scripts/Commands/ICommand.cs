using System.Collections.Generic;

namespace SysEarth.Commands
{
    public interface ICommand
    {
        string GetCommandName();

        string GetCommandDescription();

        IDictionary<string, string> GetCommandFlagDescriptions();

        IList<string> GetExampleUsages();

        bool ValidateArguments(IList<string> args);

        void ExecuteCommand(IList<string> args);

        IList<string> GetResponseMessages();
    }
}
