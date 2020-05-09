﻿namespace SysEarth.Models
{
    public class TerminalCommand
    {
        public string TerminalCommandInput { get; set; }

        public string TerminalCommandOutput { get; set; }

        public bool IsVisibleInTerminal { get; set; } = true;
    }
}
