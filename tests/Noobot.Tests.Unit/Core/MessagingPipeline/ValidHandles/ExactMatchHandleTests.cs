using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Shouldly;
using Xunit;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class ExactMatchHandleTests
    {
        [Theory]
        [InlineData("i love geoff", "i love geoff")]
        [InlineData("who wants LUNCH?", "who wants lunch?")]
        public void should_return_true_when_message_contains_text(string exactText, string message)
        {
            // given
            var handle = new ExactMatchHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeTrue();
        }
        [Theory]
        [InlineData("i love geoffs", "i love geoff")]
        [InlineData("DINNER", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new ExactMatchHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeFalse();
        }
    }
}