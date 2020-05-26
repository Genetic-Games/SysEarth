using NUnit.Framework;
using SysEarth.Parsers;
using System.Collections.Generic;

namespace SysEarth.Tests.Parser
{
    public class UserInputParserTests
    {
        private UserInputParser _userInputParser;

        [OneTimeSetUp]
        public void Setup()
        {
            _userInputParser = new UserInputParser();
        }

        [Test]
        public void NullInputTest()
        {
            var isParseInputSuccess = _userInputParser.TryParseUserInput(null, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void EmptyInputTest()
        {
            var isParseInputSuccess = _userInputParser.TryParseUserInput(string.Empty, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void SingleSpaceInputTest()
        {
            var rawInput = " ";
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void SingleTabInputTest()
        {
            var rawInput = "\t";
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void SingleReturnInputTest()
        {
            var rawInput = "\r";
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void SingleNewLineInputTest()
        {
            var rawInput = "\n";
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void AllWhiteSpaceInputTest()
        {
            var rawInput = "     \t\t\t\t\r\r\r\r\n\n\n\n";
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsFalse(isParseInputSuccess);
            Assert.IsNull(parsedInput);
        }

        [Test]
        public void RandomLettersInputTest()
        {
            var rawInput = "asdf";
            var expectedCommandName = "asdf";
            var expectedArguments = new List<string> { "asdf" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void RandomPaddedLettersInputTest()
        {
            var rawInput = "   qwerty    ";
            var expectedCommandName = "qwerty";
            var expectedArguments = new List<string> { "qwerty" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void RandomSpacedLettersInputTest()
        {
            var rawInput = " x y z ";
            var expectedCommandName = "x";
            var expectedArguments = new List<string> { "x", "y", "z" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void RandomWhiteSpacedLettersInputTest()
        {
            var rawInput = " x\ny\rz\t";
            var expectedCommandName = "x";
            var expectedArguments = new List<string> { "x", "y", "z" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void CommandNameInputTest()
        {
            var rawInput = "help";
            var expectedCommandName = "help";
            var expectedArguments = new List<string> { "help" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void CommandNameWithArgumentsInputTest()
        {
            var rawInput = "help clear";
            var expectedCommandName = "help";
            var expectedArguments = new List<string> { "help", "clear" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }

        [Test]
        public void CommandNameWithFlagsInputTest()
        {
            var rawInput = "clear --help";
            var expectedCommandName = "clear";
            var expectedArguments = new List<string> { "clear", "--help" };
            var isParseInputSuccess = _userInputParser.TryParseUserInput(rawInput, out var parsedInput);
            Assert.IsTrue(isParseInputSuccess);
            Assert.IsNotNull(parsedInput);
            Assert.AreEqual(parsedInput.CommandName, expectedCommandName);
            Assert.IsNotNull(parsedInput.Arguments);
            Assert.IsNotEmpty(parsedInput.Arguments);
            Assert.AreEqual(parsedInput.Arguments, expectedArguments);
        }
    }
}
