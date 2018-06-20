using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using LetsAgree.IOC;
using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.StandardMiddleware;
using Noobot.Core.Plugins;
using Noobot.Core.Plugins.StandardPlugins;
using SlackConnector;

namespace Noobot.Core.DependencyResolution
{
    public static class CompositionRoot<C, Cl, R, T>
        where R : IDynamicRegistration<C>, IGenericRegistration<C>, IScanningRegistraction<INoConfig>, IGenericLocatorRegistration<Cl>, IContainerGeneration<T>
        where C : ISingletonConfig, IDecoratorConfig
        where Cl : ISingletonConfig
        where T : IBasicContainer, IGenericContainer
    {
        public static void Compose(R registry, Action<R> composeMiddlewares = null)
        {
            // TODO used structuremap Scan WithDefaultConventions.  idk what those are.
            registry.RegisterAssembly(typeof(CompositionRoot<,,,>).Assembly);

            // Container used to get config etc
            var configContainer = registry.GenerateContainer();

            ComposeMiddleware(registry, composeMiddlewares, configContainer);
            
            ComposePlugins(registry);

            ComposeNoobotCore(registry, configContainer);
        }
        static void ComposeMiddleware(R registry, Action<R> configureMiddleware, IGenericContainer configContainer)
        {
            var configReader = configContainer.Resolve<IConfigReader>();

            registry.Register<IMiddleware, UnhandledMessageMiddleware>();

            if (configReader.AboutEnabled)
            {
                registry.Register<IMiddleware, AboutMiddleware>().AsDecorator();
            }

            if (configReader.StatsEnabled)
            {
                registry.Register<IMiddleware, StatsMiddleware>().AsDecorator();
            }

            configureMiddleware?.Invoke(registry);

            if (configReader.HelpEnabled)
            {
                registry.Register<IMiddleware, HelpMiddleware>().AsDecorator();
            }

            registry.Register<IMiddleware, BeginMessageMiddleware>().AsDecorator();
        }
        static void ComposePlugins(R registry)
        {
            // TODO Register stats plugin, there will be more, the ioc needs to resolve IPlugin[], is this automatic or should it be explicit?
            registry.Register<IPlugin, StatsPlugin>().AsSingleton();
        }
        static void ComposeNoobotCore(R registry, IGenericContainer configContainer)
        {
            registry.Register(() => new LazyComposition(registry.GenerateContainer()));

            if (!configContainer.TryResolve<ILog>(out var log))
            {
                registry.Register<ILog>(() => null).AsSingleton();
            }

            if(!configContainer.TryResolve<ISlackConnector>(out var slackConnector))
            {
                registry.Register<ISlackConnector,SlackConnector.SlackConnector>().AsSingleton();
            }

            registry.Register<INoobotCore, NoobotCore>().AsSingleton();
        }
    }
}