using System.Collections.Generic;

namespace SysEarth.Commands
{
    public interface ICommand
    {
        string GetCommandName();

        string GetCommandDescription();

        IDictionary<string, string> GetCommandFlagDescriptions();

        IList<string> GetExampleUsages();

        bool TryValidateArguments(out string responseMessage, params string[] args);

        string ExecuteCommand(params string[] args);
    }
}
