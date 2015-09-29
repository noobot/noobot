using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using Noobot.Domain.Slack;
using Noobot.Tests.Unit.Stubs.MessagingPipeline;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Domain.Slack
{
    [TestFixture]
    public class SlackConnectorTests
    {
        [Test]
        public void should_connect_as_expected()
        {
            // given
            var configReader = new ConfigReader();
            var pipelineManagerStub = new PipelineManagerStub();

            var connector = new SlackConnector(configReader, pipelineManagerStub);

            // when
            var task = connector.Connect();
            task.Wait();

            // then
            Assert.That(task.Result, Is.Not.Null);
        }
    }
}