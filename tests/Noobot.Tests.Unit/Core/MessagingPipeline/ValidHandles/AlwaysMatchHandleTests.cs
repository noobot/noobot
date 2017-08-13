using System;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Should;
using Xunit;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class AlwaysMatchHandleTests
    {
        [Fact]
        public void should_always_return_true()
        {
            // given
            string message = Guid.NewGuid().ToString();
            var handle = new AlwaysMatchHandle();

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            isMatch.ShouldBeTrue();
        }
    }
}