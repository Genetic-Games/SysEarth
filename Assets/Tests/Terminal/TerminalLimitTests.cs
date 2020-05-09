using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.Terminal
{
    public class TerminalLimitTests
    {
        [Test]
        public void GetDefaultInputHistoryLimitTest()
        {
            var terminalState = new TerminalState();
            var actual = terminalState.GetCommandHistoryLimit();

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual, 0);
        }

        [Test]
        public void SetInputHistoryLimitTest()
        {
            var terminalState = new TerminalState();
            var expected = 20;
            var isLimitSet = terminalState.TrySetCommandHistoryLimit(expected);
            var actual = terminalState.GetCommandHistoryLimit();

            Assert.IsTrue(isLimitSet);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDefaultInputLengthLimitTest()
        {
            var terminalState = new TerminalState();
            var actual = terminalState.GetInputLengthLimit();

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual, 0);
        }

        [Test]
        public void SetInputLengthLimitTest()
        {
            var terminalState = new TerminalState();
            var expected = 20;
            var isLimitSet = terminalState.TrySetTerminalInputLengthLimit(expected);
            var actual = terminalState.GetInputLengthLimit();

            Assert.IsTrue(isLimitSet);
            Assert.AreEqual(expected, actual);
        }
    }
}
