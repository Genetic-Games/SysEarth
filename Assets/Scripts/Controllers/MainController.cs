using SysEarth.States;
using UnityEngine;

namespace SysEarth.Controllers
{
    public class MainController : MonoBehaviour
    {
        // States
        private FileSystemState _fileSystemState;
        private TerminalState _terminalState;

        // Controllers
        private DirectoryController _directoryController;
        private FileController _fileController;
        private PermissionController _permissionController;

        // Start is called before the first frame update
        public void Start()
        {
            _fileSystemState = new FileSystemState();
            _terminalState = new TerminalState();

            _directoryController = new DirectoryController();
            _fileController = new FileController();
            _permissionController = new PermissionController();
        }

        // TODO - Check out https://github.com/sprache/Sprache for text parsing

        // Update is called once per frame
        public void Update()
        {

        }
    }
}