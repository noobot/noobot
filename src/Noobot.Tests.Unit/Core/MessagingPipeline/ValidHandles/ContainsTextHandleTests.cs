using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class ContainsTextHandleTests
    {
        [TestCase("geoff", "i love geoff")]
        [TestCase("lunch", "who wants LUNCH?")]
        public void should_return_true_when_message_contains_text(string containsText, string message)
        {
            // given
            var handle = new ContainsTextHandle(containsText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.True);
        }

        [TestCase("simon", "i love geoff")]
        [TestCase("DINNER", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string containsText, string message)
        {
            // given
            var handle = new ContainsTextHandle(containsText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.False);
        }
    }
}