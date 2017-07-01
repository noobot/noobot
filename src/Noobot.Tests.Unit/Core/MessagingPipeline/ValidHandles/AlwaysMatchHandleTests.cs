using System;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.MessagingPipeline.ValidHandles
{
    public class AlwaysMatchHandleTests
    {
        [Test]
        public void should_always_return_true()
        {
            // given
            string message = Guid.NewGuid().ToString();
            var handle = new AlwaysMatchHandle();

            // when
            bool isMatch = handle.IsMatch(message);

            // then
            Assert.That(isMatch, Is.True);
        }
    }
}