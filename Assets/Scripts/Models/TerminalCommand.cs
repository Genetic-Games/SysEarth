namespace SysEarth.Models
{
    public class TerminalCommand
    {
        public uint TerminalCommandNumber { get; set; }

        public string TerminalCommandInput { get; set; }

        public string TerminalCommandOutput { get; set; }

        public bool IsVisibleInTerminal { get; set; } = true;
    }
}
