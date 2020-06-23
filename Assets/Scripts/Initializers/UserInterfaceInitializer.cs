using SysEarth.Controllers;
using SysEarth.States;
using UnityEngine;
using UnityEngine.UI;

namespace SysEarth.Initializers
{
    public class UserInterfaceInitializer
    {
        public void InitializeConsoleText(
            UserInterfaceController userInterfaceController, 
            FileSystemState fileSystemState, 
            DirectoryController directoryController,
            Text inputText, 
            Text outputText)
        {
            Debug.Assert(inputText != null, "The input text object is not properly set.");
            Debug.Assert(outputText != null, "The output text object is not properly set.");

            // Ensure that the console text objects are empty to start (except for prompts and path metadata)
            var currentDirectory = fileSystemState.GetCurrentDirectory();
            var currentDirectoryPath = directoryController.GetDirectoryPath(currentDirectory);

            userInterfaceController.SetUserInterfaceTextWithInputPrompt(inputText, string.Empty, currentDirectoryPath);
            userInterfaceController.SetUserInterfaceText(outputText, updatedText: string.Empty);
        }
    }
}
