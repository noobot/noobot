using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Tests.Unit.Stubs.MessagingPipeline;
using Xunit;

namespace Noobot.Tests.Unit.Core.Slack
{
    public class SlackConnectorTests
    {
        [Fact]
        public async Task should_connect_as_expected()
        {
            // given
            var configReader = JsonConfigReader.DefaultLocation();
            var containerStub = new NoobotContainerStub();
            var connector = new NoobotCore(configReader, new Mock<ILogger>().Object, containerStub);

            // when
            await connector.Connect();

            // then
        }
    }
}