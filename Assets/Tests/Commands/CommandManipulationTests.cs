using NUnit.Framework;
using SysEarth.Commands;
using SysEarth.States;
using System.Collections;

namespace SysEarth.Tests.Commands
{
    public class CommandManipulationTests
    {
        private string _commandName;
        private ICommand _command;

        [OneTimeSetUp]
        public void Setup()
        {
            _commandName = "test";
            _command = new TestCommand();
        }

        [Test]
        public void NewStateHasNoCommands()
        {
            var commandState = new CommandState();
            var availableCommands = commandState.GetAvailableCommands();

            Assert.IsNotNull(availableCommands);
            Assert.IsEmpty(availableCommands);
        }

        [Test]
        public void CommandNotInStateIsUnavailable()
        {
            var commandState = new CommandState();
            var isCommandAvailable = commandState.IsCommandAvailable(_commandName);

            Assert.IsFalse(isCommandAvailable);
        }

        [Test]
        public void CanGetAvailableCommandAddedToState()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isCommandAvailable = commandState.IsCommandAvailable(_commandName);

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isCommandAvailable);
        }

        [Test]
        public void CannotGetUnavailableCommand()
        {
            var commandState = new CommandState();
            var isGetCommandSuccess = commandState.TryGetCommand(_commandName, out var retrievedCommand);

            Assert.IsFalse(isGetCommandSuccess);
            Assert.IsNull(retrievedCommand);
        }

        [Test]
        public void CanAddNewAvailableCommand()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, new TestCommand());

            Assert.IsTrue(isAddCommandSuccess);
        }

        [Test]
        public void AddedCommandIsInAvailableCommands()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var availableCommands = commandState.GetAvailableCommands();

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsNotNull(availableCommands);
            Assert.IsNotEmpty(availableCommands);
            Assert.Contains(_commandName, (ICollection) availableCommands);
        }

        [Test]
        public void CanGetAvailableCommand()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isGetCommandSuccess = commandState.TryGetCommand(_commandName, out var retrievedCommand);

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isGetCommandSuccess);
            Assert.AreSame(_command, retrievedCommand);
        }

        [Test]
        public void CannotRemoveUnavailableCommand()
        {
            var commandState = new CommandState();
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(_commandName);

            Assert.IsFalse(isRemoveCommandSuccess);
        }

        [Test]
        public void CanRemoveAvailableCommand()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(_commandName);

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isRemoveCommandSuccess);
        }

        [Test]
        public void RemovedCommandIsNotInAvailableCommands()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(_commandName);
            var availableCommands = commandState.GetAvailableCommands();

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isRemoveCommandSuccess);
            Assert.IsNotNull(availableCommands);
            Assert.IsEmpty(availableCommands);
        }

        [Test]
        public void CommandIsUnavailableAfterRemoved()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isCommandAvailableAfterAddSuccess = commandState.IsCommandAvailable(_commandName);
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(_commandName);
            var isCommandAvailableAfterRemovalSuccess = commandState.IsCommandAvailable(_commandName);

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isCommandAvailableAfterAddSuccess);
            Assert.IsTrue(isRemoveCommandSuccess);
            Assert.IsFalse(isCommandAvailableAfterRemovalSuccess);
        }

        [Test]
        public void CannotGetRemovedCommand()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, _command);
            var isGetCommandAfterAdditionSuccess = commandState.TryGetCommand(_commandName, out var retrievedCommandAfterAdd);
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(_commandName);
            var isGetCommandAfterRemovalSuccess = commandState.TryGetCommand(_commandName, out var retrievedCommandAfterRemoval);

            Assert.IsTrue(isAddCommandSuccess);
            Assert.IsTrue(isGetCommandAfterAdditionSuccess);
            Assert.IsNotNull(retrievedCommandAfterAdd);
            Assert.IsTrue(isRemoveCommandSuccess);
            Assert.IsFalse(isGetCommandAfterRemovalSuccess);
            Assert.IsNull(retrievedCommandAfterRemoval);
            Assert.AreNotEqual(retrievedCommandAfterAdd, retrievedCommandAfterRemoval);
        }
    }
}
