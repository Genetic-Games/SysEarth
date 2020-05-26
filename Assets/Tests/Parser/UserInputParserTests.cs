using NUnit.Framework;
using SysEarth.Parsers;

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
        public void PlaceholderTest()
        {
            // TODO - Write some tests for input text parsing
            Assert.True(true);
        }
    }
}
