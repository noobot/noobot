using System.Threading.Tasks;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.Logging;
using Noobot.Tests.Unit.Stubs.MessagingPipeline;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Core.Slack
{
    [TestFixture]
    public class SlackConnectorTests
    {
        [Test]
        public async Task should_connect_as_expected()
        {
            // given
            var configReader = new ConfigReader();
            var containerStub = new NoobotContainerStub();
            var connector = new NoobotCore(configReader, new EmptyLogger(), containerStub);

            // when
            await connector.Connect();

            // then
        }
    }
}