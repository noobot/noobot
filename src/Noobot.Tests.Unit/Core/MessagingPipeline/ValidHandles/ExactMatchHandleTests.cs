using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class ExactMatchHandleTests
    {
        [TestCase("i love geoff", "i love geoff")]
        [TestCase("who wants LUNCH?", "who wants lunch?")]
        public void should_return_true_when_message_contains_text(string exactText, string message)
        {
            // given
            var handle = new ExactMatchHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.True);
        }

        [TestCase("i love geoffs", "i love geoff")]
        [TestCase("DINNER", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new ExactMatchHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.False);
        }
    }
}