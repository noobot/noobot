using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using PowerAssert;
using Xunit;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class ContainsTextHandleTests
    {
        [Theory]
        [InlineData("geoff", "i love geoff")]
        [InlineData("lunch", "who wants LUNCH?")]
        public void should_return_true_when_message_contains_text(string containsText, string message)
        {
            // given
            var handle = new ContainsTextHandle(containsText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            PAssert.IsTrue(()=> isMatch);
        }

        [Theory]
        [InlineData("simon", "i love geoff")]
        [InlineData("DINNER", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string containsText, string message)
        {
            // given
            var handle = new ContainsTextHandle(containsText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            PAssert.IsTrue(() => isMatch == false);
        }
    }
}