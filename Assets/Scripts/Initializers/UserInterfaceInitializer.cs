using SysEarth.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Initializers
{
    public class UserInterfaceInitializer
    {
        public void InitializeConsoleText(UserInterfaceController userInterfaceController, Text consoleText, bool addPrompt = false)
        {
            Debug.Assert(consoleText != null, "A console text object is not properly set.");

            // Ensure that the console text is empty to start
            userInterfaceController.SetUserInterfaceText(consoleText, updatedText: string.Empty, addPrompt);
        }
    }
}
