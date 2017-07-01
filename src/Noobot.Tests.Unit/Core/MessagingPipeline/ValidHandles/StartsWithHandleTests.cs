using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class StartsWithHandleTests
    {
        [TestCase("I am", "i am lord bucket head")]
        [TestCase("what", "what should I do next?")]
        public void should_return_true_when_message_contains_text(string startsWith, string message)
        {
            // given
            var handle = new StartsWithHandle(startsWith);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.True);
        }

        [TestCase("simon", "PAUL")]
        [TestCase("something else", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new StartsWithHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.False);
        }
    }
}