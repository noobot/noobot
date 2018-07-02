using System.Threading.Tasks;
using Common.Logging;
using System.IoC.StructureMapShim;
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
            var registry = new SMRegistry();

            registry.Register<ILog, EmptyLogger>();
            registry.Register<IConfigReader>(() => new JsonConfigReader());

            CompositionRoot<IConfigSpec, ILocatorConfigSpec, IRegSpec, IContainerSpec>
                .Compose(registry);

            var container = registry.GenerateContainer();
            var noobotCore = container.Resolve<INoobotCore>();

            // when
            await noobotCore.Connect();

            // then

        }
    }
}