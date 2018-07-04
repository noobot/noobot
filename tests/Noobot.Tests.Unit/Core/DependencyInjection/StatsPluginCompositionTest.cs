using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Logging;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Noobot.Tests.Unit.Core.DependencyInjection
{
    public class StatsPluginCompositionTest
    {
        [Fact]
        public void stats_plugin_should_only_appear_once()
        {
            // given
            var registry = new ServiceCollection();

            registry.AddSingleton<ILog, EmptyLogger>();
            registry.AddSingleton<IConfigReader>(s => new JsonConfigReader());

            CompositionRoot.Compose(registry);

            var container = registry.BuildServiceProvider();

            // when
            var directStats = container.GetService<StatsPlugin>();
            var allPlugins = container.GetServices<IPlugin>();
            var statsInAllPlugins = allPlugins.Where(x => x is StatsPlugin).ToArray();

            // then
            statsInAllPlugins.Length.ShouldBe(1);
            directStats.ShouldBeSameAs(statsInAllPlugins[0]);
        }
    }
}
