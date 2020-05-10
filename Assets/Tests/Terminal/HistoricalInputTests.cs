using NUnit.Framework;
using SysEarth.Models;
using SysEarth.States;
using System.Linq;

namespace SysEarth.Tests.Terminal
{
    public class HistoricalCommandTests
    {
        [Test]
        public void GetDefaultHistoricalCommandsTest()
        {
            var terminalState = new TerminalState();
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsNotNull(previousCommands);
            Assert.IsEmpty(previousCommands);
        }

        [Test]
        public void AddHistoricalCommandTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput" , TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddHistoricalCommandSuccess);
            Assert.IsNotNull(previousCommands);
            Assert.IsNotEmpty(previousCommands);
            Assert.IsTrue(previousCommands.Contains(terminalCommand));
        }

        [Test]
        public void AddHistoricalCommandAsNullTest()
        {
            var terminalState = new TerminalState();
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(null);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalCommandSuccess);
        }

        [Test]
        public void AddHistoricalCommandWithNullInputTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = null, TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalCommandSuccess);
        }

        [Test]
        public void AddHistoricalCommandWithNullOutputTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = null };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalCommandSuccess);
        }

        [Test]
        public void AddHistoricalCommandWithEmptyInputTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = string.Empty, TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalCommandSuccess);
        }

        [Test]
        public void AddHistoricalCommandWithEmptyOutputTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = string.Empty };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalCommandSuccess);
        }

        [Test]
        public void AddHistoricalCommandWhenAtLimitTest()
        {
            var terminalState = new TerminalState();
            var underLimitTerminalCommand = new TerminalCommand { TerminalCommandInput = "underLimitInput", TerminalCommandOutput = "underLimitOutput" };
            var overLimitTerminalCommand = new TerminalCommand { TerminalCommandInput = "overLimitInput", TerminalCommandOutput = "overLimitOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(1);
            var isUnderLimitAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(underLimitTerminalCommand);
            var isOverLimitAddHistoricalCommandSuccess = terminalState.TryAddHistoricalCommand(overLimitTerminalCommand);
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isUnderLimitAddHistoricalCommandSuccess);
            Assert.IsFalse(isOverLimitAddHistoricalCommandSuccess);
            Assert.IsTrue(previousCommands.Contains(underLimitTerminalCommand));
            Assert.IsFalse(previousCommands.Contains(overLimitTerminalCommand));
        }

        [Test]
        public void RemoveHistoricalCommandWhenAtLimitTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(1);
            var isAddSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            var isRemoveSuccess = terminalState.TryRemoveOldestHistoricalCommand();
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsTrue(isRemoveSuccess);
            Assert.IsFalse(previousCommands.Contains(terminalCommand));
            Assert.IsEmpty(previousCommands);
        }

        [Test]
        public void DoNotRemoveHistoricalCommandWhenUnderLimitTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(5);
            var isAddSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            var isRemoveSuccess = terminalState.TryRemoveOldestHistoricalCommand();
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsFalse(isRemoveSuccess);
            Assert.IsTrue(previousCommands.Contains(terminalCommand));
            Assert.IsNotEmpty(previousCommands);
        }

        [Test]
        public void ClearHistoricalCommandTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            terminalState.ClearPreviousCommands();
            var previousCommands = terminalState.GetPreviousTerminalCommands();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsFalse(previousCommands.Contains(terminalCommand));
            Assert.IsEmpty(previousCommands);
        }

        [Test]
        public void HistoricalCommandIsVisibleByDefaultTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            var previousCommands = terminalState.GetPreviousTerminalCommands();
            var previousCommand = previousCommands.FirstOrDefault();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsTrue(previousCommand.IsVisibleInTerminal);
        }

        [Test]
        public void HideHistoricalCommandTest()
        {
            var terminalState = new TerminalState();
            var terminalCommand = new TerminalCommand { TerminalCommandInput = "testInput", TerminalCommandOutput = "testOutput" };
            var isHistoryLimitSet = terminalState.TrySetCommandHistoryLimit(10);
            var isAddSuccess = terminalState.TryAddHistoricalCommand(terminalCommand);
            terminalState.HidePreviousCommands();
            var previousCommands = terminalState.GetPreviousTerminalCommands();
            var previousCommand = previousCommands.FirstOrDefault();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsFalse(previousCommand.IsVisibleInTerminal);
        }
    }
}
