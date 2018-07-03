using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Logging;
using Xunit;

namespace Noobot.Tests.Unit.Core.Slack
{
    public class SlackConnectorTests
    {
        [Fact]
        public async Task should_connect_as_expected()
        {
            // given
            var registry = new ServiceCollection();

            registry.AddSingleton<ILog, EmptyLogger>();
            registry.AddSingleton<IConfigReader>(s => new JsonConfigReader());

            CompositionRoot.Compose(registry);

            var container = registry.BuildServiceProvider();
            var noobotCore = container.GetService<INoobotCore>();

            // when
            await noobotCore.Connect();

            // then

        }
    }
}