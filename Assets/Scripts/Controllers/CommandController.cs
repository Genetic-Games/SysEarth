using SysEarth.Commands;
using System.Collections.Generic;

namespace SysEarth.Controllers
{
    public class CommandController
    {
        private IDictionary<string, ICommand> commandMap;

        public CommandController()
        {
            commandMap = new Dictionary<string, ICommand>();
        }

        public bool AddCommand(string commandName, ICommand command)
        {
            commandMap.Add(commandName, command);
            return commandMap.ContainsKey(commandName);
        }

        public bool RemoveCommand(string commandName)
        {
            return commandMap.Remove(commandName);
        }

        public ICollection<string> GetAvailableCommands()
        {
            return commandMap.Keys;
        }
    }
}
