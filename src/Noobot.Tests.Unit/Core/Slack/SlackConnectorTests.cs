using Noobot.Core;
using Noobot.Core.Logging;
using Noobot.Runner.Configuration;
using Noobot.Tests.Unit.Stubs.MessagingPipeline;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.Slack
{
    [TestFixture]
    public class SlackConnectorTests
    {
        [Test]
        public void should_connect_as_expected()
        {
            // given
            var configReader = new ConfigReader();
            var containerStub = new NoobotContainerStub();
            var connector = new NoobotCore(configReader, new EmptyLog(), containerStub);

            // when
            var task = connector.Connect();
            task.Wait();

            // then

        }
    }
}