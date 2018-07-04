using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using SlackConnector;
using Microsoft.Extensions.DependencyInjection;

namespace Noobot.Core.DependencyResolution
{
    public static class CompositionRoot
    {
        // So long as .BuildServiceProvider isn't used by us, this can count as abstract IoC
        public static void Compose(IServiceCollection serviceCollection, Func<IServiceCollection, IServiceProvider> buildCustomServiceProvider = null)
        {
            // Container used to get config etc
            ComposeMiddleware(serviceCollection, buildCustomServiceProvider ?? ServiceCollectionContainerBuilderExtensions.BuildServiceProvider);
            
            ComposePlugins(serviceCollection);

            ComposeNoobotCore(serviceCollection);
        }
        static void ComposeMiddleware(IServiceCollection registry, Func<IServiceCollection, IServiceProvider> buildCustomServiceProvider)
        {
            var configProvider = buildCustomServiceProvider(registry);
            var configReader = configProvider.GetRequiredService<IConfigReader>();

            if (configReader.AboutEnabled)
            {
                registry.AddSingleton<IMiddleware, AboutMiddleware>();
            }

            if (configReader.StatsEnabled)
            {
                registry.AddSingleton<IMiddleware, StatsMiddleware>();
            }

            if (configReader.HelpEnabled)
            {
                registry.AddSingleton<IMiddleware, HelpMiddleware>();
            }
        }
        static void ComposePlugins(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<StatsPlugin>();
            serviceCollection.AddSingleton<IPlugin>(s => s.GetService<StatsPlugin>());
        }
        static void ComposeNoobotCore(IServiceCollection serviceProvider)
        {
            serviceProvider.AddSingleton(s => new LazyComposition(s));
            if (!serviceProvider.Any(x=>x.ServiceType == typeof(ILog)))
            {
                serviceProvider.AddSingleton<ILog>(s => null);
            }

            if (!serviceProvider.Any(x => x.ServiceType == typeof(ISlackConnector)))
            {
                serviceProvider.AddSingleton<ISlackConnector,SlackConnector.SlackConnector>();
            }

            serviceProvider.AddSingleton<INoobotCore, NoobotCore>();
        }
    }
}