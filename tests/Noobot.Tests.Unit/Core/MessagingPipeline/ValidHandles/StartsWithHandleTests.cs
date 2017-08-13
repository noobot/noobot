using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Should;
using Xunit;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class StartsWithHandleTests
    {
        [Theory]
        [InlineData("I am", "i am lord bucket head")]
        [InlineData("what", "what should I do next?")]
        public void should_return_true_when_message_contains_text(string startsWith, string message)
        {
            // given
            var handle = new StartsWithHandle(startsWith);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeTrue();
        }

        [Theory]
        [InlineData("simon", "PAUL")]
        [InlineData("something else", "who wants lunch?")]
        public void should_return_false_when_message_doesnt_contains_text(string exactText, string message)
        {
            // given
            var handle = new StartsWithHandle(exactText);

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeFalse();
        }
    }
}