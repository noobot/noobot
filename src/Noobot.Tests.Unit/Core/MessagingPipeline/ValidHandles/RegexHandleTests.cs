using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class RegexHandleTests
    {
        [TestCase(".*", "i love geoff")]
        [TestCase("simon", "SIMON")]
        public void should_return_true_when_message_contains_text(string regex, string message)
        {
            // given
            var handle = new RegexHandle(regex);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.True);
        }

        [TestCase("simon", "PAUL")]
        [TestCase(@"^\d$", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new RegexHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.False);
        }
    }
}