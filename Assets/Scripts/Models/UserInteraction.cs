namespace SysEarth.Models
{
    public class UserInteraction
    {
        public bool IsInputSubmitted { get; set; }

        public string SubmittedInput { get; set; }

        public bool IsInputModified { get; set; }

        public string ModifiedInput { get; set; }

        public bool IsOutputModified { get; set; }

        public string ModifiedOutput { get; set; }
    }
}
