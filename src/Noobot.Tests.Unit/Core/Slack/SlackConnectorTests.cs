using Noobot.Core;
using Noobot.Core.Configuration;
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
            var pipelineFactory = new PipelineFactoryStub();

            var connector = new NoobotCore(configReader, pipelineFactory);

            // when
            var task = connector.Connect();
            task.Wait();

            // then

        }
    }
}