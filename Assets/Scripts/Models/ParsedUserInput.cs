using System.Collections.Generic;

namespace SysEarth.Models
{
    public class ParsedUserInput
    {
        public string CommandName { get; set; }

        public IList<string> Arguments { get; set; }
    }
}
