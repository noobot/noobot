using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Should;
using Xunit;


namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class RegexHandleTests
    {
        [Theory]
        [InlineData(".*", "i love geoff")]
        [InlineData("simon", "SIMON")]
        public void should_return_true_when_message_contains_text(string regex, string message)
        {
            // given
            var handle = new RegexHandle(regex);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeTrue();
        }

        [InlineData("simon", "PAUL")]
        [InlineData(@"^\d$", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new RegexHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeFalse();
        }
    }
}