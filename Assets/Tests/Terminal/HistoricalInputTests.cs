using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.Terminal
{
    public class HistoricalInputTests
    {
        [Test]
        public void GetDefaultHistoricalInputsTest()
        {
            var terminalState = new TerminalState();
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsNotNull(previousInputs);
            Assert.IsEmpty(previousInputs);
        }

        [Test]
        public void AddHistoricalInputTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(10);
            var isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(expected);
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddHistoricalInputSuccess);
            Assert.IsNotNull(previousInputs);
            Assert.IsNotEmpty(previousInputs);
            Assert.That(previousInputs.Contains(expected));
        }

        [Test]
        public void AddHistoricalInputAsNullTest()
        {
            var terminalState = new TerminalState();
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(10);
            var isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(null);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalInputSuccess);
        }

        [Test]
        public void AddHistoricalInputAsEmptyTest()
        {
            var terminalState = new TerminalState();
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(10);
            var isAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(string.Empty);

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsFalse(isAddHistoricalInputSuccess);
        }

        [Test]
        public void AddHistoricalInputWhenAtLimitTest()
        {
            var terminalState = new TerminalState();
            var underLimitInput = "under limit";
            var overLimitInput = "over limit";
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(1);
            var isUnderLimitAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(underLimitInput);
            var isOverLimitAddHistoricalInputSuccess = terminalState.TryAddHistoricalInput(overLimitInput);
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isUnderLimitAddHistoricalInputSuccess);
            Assert.IsFalse(isOverLimitAddHistoricalInputSuccess);
            Assert.That(previousInputs.Contains(underLimitInput));
            Assert.IsFalse(previousInputs.Contains(overLimitInput));
        }

        [Test]
        public void RemoveHistoricalInputWhenAtLimitTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(1);
            var isAddSuccess = terminalState.TryAddHistoricalInput(expected);
            var isRemoveSuccess = terminalState.TryRemoveOldestHistoricalInput();
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsTrue(isRemoveSuccess);
            Assert.IsFalse(previousInputs.Contains(expected));
            Assert.IsEmpty(previousInputs);
        }

        [Test]
        public void DoNotRemoveHistoricalInputWhenUnderLimitTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(5);
            var isAddSuccess = terminalState.TryAddHistoricalInput(expected);
            var isRemoveSuccess = terminalState.TryRemoveOldestHistoricalInput();
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsFalse(isRemoveSuccess);
            Assert.That(previousInputs.Contains(expected));
            Assert.IsNotEmpty(previousInputs);
        }

        [Test]
        public void ClearHistoricalInputTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isHistoryLimitSet = terminalState.TrySetInputHistoryLimit(10);
            var isAddSuccess = terminalState.TryAddHistoricalInput(expected);
            terminalState.ClearPreviousInputs();
            var previousInputs = terminalState.GetPreviousTerminalInputs();

            Assert.IsTrue(isHistoryLimitSet);
            Assert.IsTrue(isAddSuccess);
            Assert.IsFalse(previousInputs.Contains(expected));
            Assert.IsEmpty(previousInputs);
        }
    }
}
