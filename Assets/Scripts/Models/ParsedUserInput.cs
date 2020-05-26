using System.Collections.Generic;

namespace SysEarth.Models
{
    public class ParsedUserInput
    {
        public string CommandName { get; set; } = string.Empty;

        // Note - The Arguments list contains the CommandName as its first element, similar in function to CLI programs
        public IList<string> Arguments { get; set; } = new List<string>();
    }
}
