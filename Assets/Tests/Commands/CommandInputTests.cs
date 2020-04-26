using NUnit.Framework;
using SysEarth.Commands;
using SysEarth.States;

namespace SysEarth.Tests.Commands
{
    public class CommandInputTests
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
        public void NullCommandIsUnavailable()
        {
            var commandState = new CommandState();
            var commandIsAvailable = commandState.IsCommandAvailable(null);

            Assert.IsFalse(commandIsAvailable);
        }

        [Test]
        public void EmptyStringCommandIsUnavailable()
        {
            var commandState = new CommandState();
            var commandIsAvailable = commandState.IsCommandAvailable(string.Empty);

            Assert.IsFalse(commandIsAvailable);
        }

        [Test]
        public void NullCommandCannotBeRetrieved()
        {
            var commandState = new CommandState();
            var isGetCommandSuccess = commandState.TryGetCommand(null, out var retrievedCommand);

            Assert.IsFalse(isGetCommandSuccess);
            Assert.IsNull(retrievedCommand);
        }

        [Test]
        public void EmptyStringCommandCannotBeRetrieved()
        {
            var commandState = new CommandState();
            var isGetCommandSuccess = commandState.TryGetCommand(string.Empty, out var retrievedCommand);

            Assert.IsFalse(isGetCommandSuccess);
            Assert.IsNull(retrievedCommand);
        }

        [Test]
        public void NullCommandNameCannotBeAdded()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(null, _command);

            Assert.IsFalse(isAddCommandSuccess);
        }

        [Test]
        public void EmptyStringCommandNameCannotBeAdded()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(string.Empty, _command);

            Assert.IsFalse(isAddCommandSuccess);
        }

        [Test]
        public void NullCommandValueCannotBeAdded()
        {
            var commandState = new CommandState();
            var isAddCommandSuccess = commandState.TryAddAvailableCommand(_commandName, null);

            Assert.IsFalse(isAddCommandSuccess);
        }


        [Test]
        public void NullCommandCannotBeRemoved()
        {
            var commandState = new CommandState();
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(null);

            Assert.IsFalse(isRemoveCommandSuccess);
        }

        [Test]
        public void EmptyStringCommandCannotBeRemoved()
        {
            var commandState = new CommandState();
            var isRemoveCommandSuccess = commandState.TryRemoveAvailableCommand(string.Empty);

            Assert.IsFalse(isRemoveCommandSuccess);
        }
    }
}
