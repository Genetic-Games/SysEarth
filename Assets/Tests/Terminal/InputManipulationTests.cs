using NUnit.Framework;
using SysEarth.States;

namespace SysEarth.Tests.Terminal
{
    public class InputManipulationTests
    {
        [Test]
        public void ValidInputTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(10);
            var isInputValid = terminalState.TryValidateInput(expected, out var validInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsTrue(isInputValid);
            Assert.AreEqual(expected, validInput);
        }

        [Test]
        public void InvalidInputLengthTest()
        {
            var terminalState = new TerminalState();
            var expected = "test";
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(1);
            var isInputValid = terminalState.TryValidateInput(expected, out var validInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputValid);
            Assert.IsNull(validInput);
            Assert.AreNotEqual(expected, validInput);
        }

        [Test]
        public void InvalidInputNullTest()
        {
            var terminalState = new TerminalState();
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(1);
            var isInputValid = terminalState.TryValidateInput(null, out var validInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputValid);
            Assert.IsNull(validInput);
        }

        [Test]
        public void InvalidInputEmptyTest()
        {
            var terminalState = new TerminalState();
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(1);
            var isInputValid = terminalState.TryValidateInput(string.Empty, out var validInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputValid);
            Assert.IsNull(validInput);
        }

        [Test]
        public void TruncatedInputTest()
        {
            var terminalState = new TerminalState();
            var input = "test";
            var lengthLimit = 1;
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(lengthLimit);
            var isInputTruncated = terminalState.TryTruncateInput(input, out var truncatedInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsTrue(isInputTruncated);
            Assert.AreNotEqual(input, truncatedInput);
            Assert.AreEqual(truncatedInput.Length, lengthLimit);
        }

        [Test]
        public void NonTruncatedInputTest()
        {
            var terminalState = new TerminalState();
            var input = "test";
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(10);
            var isInputTruncated = terminalState.TryTruncateInput(input, out var truncatedInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputTruncated);
            Assert.IsNull(truncatedInput);
        }

        [Test]
        public void NonTruncatedNullInputTest()
        {
            var terminalState = new TerminalState();
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(1);
            var isInputTruncated = terminalState.TryTruncateInput(null, out var truncatedInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputTruncated);
            Assert.IsNull(truncatedInput);
        }

        [Test]
        public void NonTruncatedEmptyInputTest()
        {
            var terminalState = new TerminalState();
            var isInputLengthSet = terminalState.TrySetInputLengthLimit(1);
            var isInputTruncated = terminalState.TryTruncateInput(string.Empty, out var truncatedInput);

            Assert.IsTrue(isInputLengthSet);
            Assert.IsFalse(isInputTruncated);
            Assert.IsNull(truncatedInput);
        }
    }
}
