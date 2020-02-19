using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.Terminal
{
    public class CurrentInputTests
    {
        [Test]
        public void GetDefaultCurrentInputTest()
        {
            var terminalState = new TerminalState();
            var actual = terminalState.GetCurrentInput();

            Assert.IsNull(actual);
        }

        [Test]
        public void SetCurrentInputTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isLengthLimitSet = terminalState.TrySetInputLengthLimit(10);
            var isInputSet = terminalState.TrySetCurrentInput(expected);
            var actual = terminalState.GetCurrentInput();

            Assert.IsTrue(isLengthLimitSet);
            Assert.IsTrue(isInputSet);
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void SetCurrentInputAsNullTest()
        {
            var terminalState = new TerminalState();
            var isInputSet = terminalState.TrySetCurrentInput(null);

            Assert.IsFalse(isInputSet);
        }

        [Test]
        public void SetCurrentInputAsEmptyTest()
        {
            var terminalState = new TerminalState();
            var isInputSet = terminalState.TrySetCurrentInput(string.Empty);

            Assert.IsFalse(isInputSet);
        }

        [Test]
        public void SetCurrentInputOverLengthLimitTest()
        {
            var terminalState = new TerminalState();
            var stringOverLengthLimit = "this is a really long test";
            var isLengthLimitSet = terminalState.TrySetInputLengthLimit(10);
            var isInputSet = terminalState.TrySetCurrentInput(stringOverLengthLimit);

            Assert.IsTrue(isLengthLimitSet);
            Assert.IsFalse(isInputSet);
        }

        [Test]
        public void ClearCurrentInputTest()
        {
            var terminalState = new TerminalState();
            var previousInput = "exists";
            var isLengthLimitSet = terminalState.TrySetInputLengthLimit(10);
            var isInputSet = terminalState.TrySetCurrentInput(previousInput);
            terminalState.ClearCurrentInput();
            var actual = terminalState.GetCurrentInput();

            Assert.IsTrue(isLengthLimitSet);
            Assert.IsTrue(isInputSet);
            Assert.IsNull(actual);
        }
    }
}
